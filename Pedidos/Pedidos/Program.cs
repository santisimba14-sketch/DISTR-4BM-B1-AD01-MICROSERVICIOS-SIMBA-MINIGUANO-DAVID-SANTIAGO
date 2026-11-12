using Microsoft.EntityFrameworkCore;
using Pedidos.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar controladores
builder.Services.AddControllers();

// Conectar con la base de datos PedidosDB usando PedidosConnection
builder.Services.AddDbContext<PedidosDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PedidosConnection")));

// Configurar comunicacion con CatalogoApi
builder.Services.AddHttpClient("CatalogoApi", cliente =>
{
    var direccion = builder.Configuration["Servicios:CatalogoApi"]
        ?? throw new InvalidOperationException("Falta configurar servicios:CatalogoApi");

    cliente.BaseAddress = new Uri(direccion);
    cliente.Timeout = TimeSpan.FromSeconds(5);
});

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PedidosDB API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
