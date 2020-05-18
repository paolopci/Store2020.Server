using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System.Collections.Generic;

namespace ServerApp.Controllers
{
  [Route("api/suppliers")]
  [ApiController]
  public class SupplierValuesController : ControllerBase
  {
    private readonly DataContext _context;

    public SupplierValuesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IEnumerable<Supplier> GetSupplier()
    {
      return _context.Suppliers;
    }

    [HttpPost]
    public IActionResult CreateSupplier([FromBody] SupplierData newSupplier)
    {
      if (ModelState.IsValid)
      {
        Supplier s = newSupplier.Supplier;
        _context.Add(s);
        _context.SaveChanges();
        return Ok(s.SupplierId);
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpPut("{id}")]
    public IActionResult ReplaceSupplier(long id, [FromBody] SupplierData suppData)
    {
      if (ModelState.IsValid)
      {
        Supplier s = suppData.Supplier;
        s.SupplierId = id;
        _context.Update(s);
        _context.SaveChanges();
        return Ok();
      }

      return BadRequest(ModelState);
    }

    [HttpDelete("{id}")]
    public void DeleteSupplier(long id)
    {
      _context.Remove(new Supplier {SupplierId = id});
      _context.SaveChanges();
    }
  }
}