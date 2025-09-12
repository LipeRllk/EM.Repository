using EM.Montador.PDF;
using EM.Repository;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllersWithViews();

// Registrar serviço de conexão com banco
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

// Registrar repositórios
builder.Services.AddScoped<CidadeRepository>();
builder.Services.AddScoped<AlunoRepository>();

// Registrar o serviço PDF
builder.Services.AddScoped<IServicePDF, ServicePDF>();

var app = builder.Build();

// Configure o pipeline de requisições HTTP
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