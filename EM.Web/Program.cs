using EM.Montador.PDF;
using EM.Repository;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao cont�iner
builder.Services.AddControllersWithViews();

// Registrar servi�o de conex�o com banco
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

// Registrar reposit�rios
builder.Services.AddScoped<CidadeRepository>();
builder.Services.AddScoped<AlunoRepository>();

// Registrar o servi�o PDF
builder.Services.AddScoped<IServicePDF, ServicePDF>();

var app = builder.Build();

// Configure o pipeline de requisi��es HTTP
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