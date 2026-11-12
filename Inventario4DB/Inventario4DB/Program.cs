using Microsoft.EntityFrameworkCore;
using Inventario4DB.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar controladores
builder.Services.AddControllers();

// Conectar con la base de datos Inventario4DB usando InventarioConnection
builder.Services.AddDbContext<InventarioDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventarioConnection")));

// Configurar comunicacion con CatalogoApi
builder.Services.AddHttpClient("CatalogoApi", cliente =>
{
    var direccion = builder.Configuration["Servicios:CatalogoApi"]
        ?? throw new InvalidOperationException("Falta configurar servicios:CatalogoApi");

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventario4DB API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
