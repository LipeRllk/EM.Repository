using EM.Domain.Context;
using EM.Domain.Models;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<EscolaDbContext>(options =>
            options.UseFirebird(builder.Configuration.GetConnectionString("FirebirdConnection"))
        );

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
