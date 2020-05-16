using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

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
      return _context.Products.Find(id);
    }
  }
}