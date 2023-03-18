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
// e�er ProductRepositoryWithCacheDecorator i�inde IProductRepository g�r�rsen;
// ProductRepository'den bir �rnek ver.
// e�er bunun d���nda bir yerde IProductRepository g�r�rsen;
// ProductRepositoryWithCacheDecorator'den bir �rnek ver.
// Decorator Design Pattern
builder.Services.AddScoped<IProductRepository>(sp =>
{
    // bir tane AppDbContext �rne�i ver;
    var appDbContext = sp.GetRequiredService<AppDbContext>();
    // bir tane ProductRepository olu�tur ve istedi�i contexti yukar� ald���m�z �rnek olarak ver
    var productRepository = new ProductRepository(appDbContext);
    // bir tane RedisService �rne�i ver
    var redisService = sp.GetRequiredService<RedisService>();
    // geriye bir tane ProductRepositoryWithCacheDecorator d�n ve istediklerini yukar�daki �rnekler olarak ver
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
