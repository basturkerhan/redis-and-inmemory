using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RedisInMemory.InMemoryApp.Web.Models;

namespace RedisInMemory.InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {

        private readonly IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //// 1. Yol
            //if(string.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}

            // 2. Yol
            if(!_memoryCache.TryGetValue("zaman", out string zamancache))
            {
                MemoryCacheEntryOptions options = new()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                    SlidingExpiration = TimeSpan.FromSeconds(10),
                    Priority = CacheItemPriority.High, // bu data benim için önemli, silme sırasında bunu önce silme, sonlara bırak diyoruz
                };

                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    // bir veri silindiği zaman neden silindiğine dair sebebini öğrenebiliriz
                    _memoryCache.Set("callback", $"{key}->{value} => sebep:{reason}");
                });

                _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
                //ViewBag.zaman = zamancache;
            }

            Product p = new()
            {
                Id = 1,
                Amount = 1,
                Name = "Test",
            };
            _memoryCache.Set<Product>("ürün:1", p);

            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.Remove("zaman");
            // Almaya çalış eğer yoksa oluştur
            //_memoryCache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});

            _memoryCache.TryGetValue<string>("zaman", out string zamancache);
            _memoryCache.TryGetValue<string>("callback", out string callbackcache);
            _memoryCache.TryGetValue<Product>("ürün:1", out Product productcache);

            ViewBag.zaman = zamancache;
            ViewBag.callback = callbackcache;
            ViewBag.product = productcache;
            return View();
        }
    }
}
