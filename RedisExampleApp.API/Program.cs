using Microsoft.EntityFrameworkCore;
using RedisInMemory.RedisExampleApp.API.Models;
using RedisInMemory.RedisExampleApp.API.Repositories;
using RedisInMemory.RedisExampleApp.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region addProductRepository
// eðer ProductRepositoryWithCacheDecorator içinde IProductRepository görürsen;
// ProductRepository'den bir örnek ver.
// eðer bunun dýþýnda bir yerde IProductRepository görürsen;
// ProductRepositoryWithCacheDecorator'den bir örnek ver.
// Decorator Design Pattern
builder.Services.AddScoped<IProductRepository>(sp =>
{
    // bir tane AppDbContext örneði ver;
    var appDbContext = sp.GetRequiredService<AppDbContext>();
    // bir tane ProductRepository oluþtur ve istediði contexti yukarý aldýðýmýz örnek olarak ver
    var productRepository = new ProductRepository(appDbContext);
    // bir tane RedisService örneði ver
    var redisService = sp.GetRequiredService<RedisService>();
    // geriye bir tane ProductRepositoryWithCacheDecorator dön ve istediklerini yukarýdaki örnekler olarak ver
    return new ProductRepositoryWithCacheDecorator(productRepository, redisService);
});
#endregion

#region addDbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("localdb");
});
#endregion

#region addRedisService
builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration["CacheOptions:Url"]);
});
#endregion

var app = builder.Build();

#region inMemoryDbUp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}
#endregion

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
