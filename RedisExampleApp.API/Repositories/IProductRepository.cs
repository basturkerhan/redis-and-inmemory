using RedisInMemory.RedisExampleApp.API.Models;

namespace RedisInMemory.RedisExampleApp.API.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetAsync(int id);
        Task<Product> CreateAsync(Product product);
    }
}
