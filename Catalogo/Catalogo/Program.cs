using Microsoft.EntityFrameworkCore;
using Catalogo.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar controladores
builder.Services.AddControllers();

// Conectar con la base de datos CatalogoDB usando CatalogoConnection
builder.Services.AddDbContext<CatalogoDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogoConnection")));

// Configurar comunicacion con InventarioApi
builder.Services.AddHttpClient("InventarioApi", cliente =>
{
    var direccion = builder.Configuration["Servicios:InventarioApi"]
        ?? throw new InvalidOperationException("Falta configurar servicios:InventarioApi");

    cliente.BaseAddress = new Uri(direccion);
    cliente.Timeout = TimeSpan.FromSeconds(5);
});

// Configurar Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogoDB API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
