using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System.Collections.Generic;
using System.Linq;
using ServerApp.Models.BindingTargets;

namespace ServerApp.Controllers
{
  [Route("api/products")]
  [ApiController]
  public class ProductValuesController : ControllerBase
  {
    private readonly DataContext _context;

    public ProductValuesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet("{id}")]
    public Product GetProduct(long id)
    {
      //System.Threading.Thread.Sleep(5000);
      Product result = _context.Products.Include(p => p.Supplier)
        .ThenInclude(s => s.Products)
        .Include(p => p.Ratings)
        .FirstOrDefault(p => p.ProductId == id);

      if (result != null)
      {
        if (result.Supplier != null)
        {
          result.Supplier.Products = result.Supplier.Products.Select(p =>
            new Product
            {
              ProductId = p.ProductId,
              Name = p.Name,
              Category = p.Category,
              Description = p.Description,
              Price = p.Price
            });
        }

        if (result.Ratings != null)
        {
          foreach (Rating r in result.Ratings)
          {
            r.Product = null;
          }
        }
      }

      return result;
    }

    [HttpGet]
    public IActionResult GetProducts(string category, string search,
      bool related = false, bool metadata = false)
    {
      IQueryable<Product> query = _context.Products;

      if (!string.IsNullOrWhiteSpace(category))
      {
        string catLower = category.ToLower();
        query = query.Where(p => p.Category.ToLower().Contains(catLower));
      }

      if (!string.IsNullOrWhiteSpace(search))
      {
        string searchLower = search.ToLower();
        query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                                 || p.Description.ToLower().Contains(searchLower));
      }


      if (related)
      {
        query = query.Include(p => p.Supplier).Include(p => p.Ratings);
        List<Product> data = query.ToList();
        data.ForEach(p =>
        {
          if (p.Supplier != null)
          {
            p.Supplier.Products = null;
          }

          if (p.Ratings != null)
          {
            p.Ratings.ForEach(r => r.Product = null);
          }
        });
        return metadata ? CreateMetadata(data) : Ok(data);
      }

      return metadata ? CreateMetadata(query) : Ok(query);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] ProductData newProduct)
    {
      if (ModelState.IsValid)
      {
        Product p = newProduct.Product;
        if (p.Supplier != null && p.Supplier.SupplierId != 0)
        {
          _context.Attach(p.Supplier);
        }

        _context.Add(p);
        _context.SaveChanges();
        return Ok(p.ProductId);
      }

      return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public IActionResult ReplaceProduct(long id, [FromBody] ProductData prodData)
    {
      if (ModelState.IsValid)
      {
        Product p = prodData.Product;
        p.ProductId = id;
        if (p.Supplier != null && p.Supplier.SupplierId != 0)
        {
          _context.Attach(p.Supplier);
        }

        _context.Update(p);
        _context.SaveChanges();
        return Ok();
      }

      return BadRequest(ModelState);
    }

    [HttpDelete("{id}")]
    public void DeleteProduct(long id)
    {
      _context.Products.Remove(new Product {ProductId = id});
      _context.SaveChanges();
    }

    private IActionResult CreateMetadata(IEnumerable<Product> products)
    {
      return Ok(new
      {
        data = products,
        categories = _context.Products.Select(p => p.Category).Distinct().OrderBy(c => c)
      });
    }
  }
}