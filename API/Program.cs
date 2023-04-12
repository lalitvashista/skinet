using Infrastructure.Data;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddDbContext<StoreContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;
var loggerfactory = services.GetRequiredService<ILoggerFactory>();
try{

    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context,loggerfactory);
}
catch(Exception ex)
{
    var logger = loggerfactory.CreateLogger<Program>();
    logger.LogError(ex,"An error occured during migration");
}
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
