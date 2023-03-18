using RedisInMemory.RedisExampleApp.API.Models;
using RedisInMemory.RedisExampleApp.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisInMemory.RedisExampleApp.API.Repositories
{
    public class ProductRepositoryWithCacheDecorator : IProductRepository
    {
        private readonly IProductRepository _repository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepository;
        private const string productKey  = "productCaches"; 

        public ProductRepositoryWithCacheDecorator(IProductRepository repository, RedisService redisService)
        {
            _repository = repository;
            _redisService = redisService;
            _cacheRepository = redisService.GetDatabase(2);
        }

        private async Task<List<Product>> LoadToCacheFromDbAsync()
        {
            var products = await _repository.GetAllAsync();
            products.ForEach(product =>
            {
                _cacheRepository.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(product));
            });

            return products;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            Product newProduct = await _repository.CreateAsync(product);

            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                await _cacheRepository.HashSetAsync(productKey, newProduct.Id, JsonSerializer.Serialize(newProduct));
            }

            return newProduct;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            if(!await _cacheRepository.KeyExistsAsync(productKey))
            {
                // eğer cache üzerinde data yoksa veritabanından getir ve ayrıca cache üzerine yükle
                return await LoadToCacheFromDbAsync();
            }
            // eğer cache üzerinde veriler varsa;
            List<Product> products = new();
            var cacheProducts = await _cacheRepository.HashGetAllAsync(productKey);
            foreach (var item in cacheProducts.ToList())
            {
                products.Add(JsonSerializer.Deserialize<Product>(item.Value));
            }

            return products;
        }

        public async Task<Product?> GetAsync(int id)
        {
            if(await _cacheRepository.KeyExistsAsync(productKey))
            {
                var product = await _cacheRepository.HashGetAsync(productKey, id);
                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
            }

            List<Product> products = await LoadToCacheFromDbAsync();
            return products.FirstOrDefault(x => x.Id == id);
        }
    }
}
