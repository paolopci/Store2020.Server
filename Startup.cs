using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices.Extensions;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using Microsoft.OpenApi.Models;

namespace ServerApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
      services.AddDbContext<DataContext>(
        opts => opts.UseSqlServer(connectionString));

// Chapter 5
      services.AddControllersWithViews()
              .AddJsonOptions(o=> { o.JsonSerializerOptions.IgnoreNullValues = true; });

      services.AddRazorPages();
      services.AddSwaggerGen(opt =>
      {
        opt.SwaggerDoc("v1", new OpenApiInfo {Title = "Store2020 API", Version = "v1"});
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
         // Chapter 7
         endpoints.MapControllerRoute(
           name: "angular_fallback",
           pattern: "{target:regex(store|cart)}/{*catchall}",
           defaults: new {controller = "Home", action = "Index"}
         );
      });
     

      // Chapter 5 install Swagger
      app.UseSwagger();
      app.UseSwaggerUI(opts =>
      {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Store2020 API");
      });




      app.UseSpa(spa =>
      {
        string strategy = Configuration.GetValue<string>("DevTools:ConnectionStrategy");
        if (strategy == "proxy")
        {
          spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4200");
        }
        else if (strategy == "managed")
        {
          spa.Options.SourcePath = "../ClientApp";
          spa.UseAngularCliServer("start");
        }
      });

      SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
    }
  }
}