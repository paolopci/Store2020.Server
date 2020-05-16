using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Models;

namespace ServerApp.Controllers
{
  public class HomeController : Controller
  {
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
      _context = context;
    }

    public IActionResult Index()
    {
      return View(_context.Products.First());
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
  }
}