using EM.Montador.PDF;
using EM.Repository;
using EM.Repository.Extensions;
using EM.Domain.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

builder.Services.AddRepositories(typeof(CidadeRepository).Assembly);

builder.Services.AddScoped<IServicePDF, ServicePDF>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();