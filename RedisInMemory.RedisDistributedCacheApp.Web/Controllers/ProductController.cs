using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisInMemory.RedisDistributedCacheApp.Web.Models;
using System.Text;

namespace RedisInMemory.RedisDistributedCacheApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                SlidingExpiration = TimeSpan.FromSeconds(20)
            };
            // CACHING STRING
            //await _distributedCache.SetStringAsync("isim","erhan", options);


            // CACHING ENTITY
            Product p = new()
            {
                Id = 1,
                Amount = 10,
                Name = "Ürün Deneme"
            };

            string jsonProduct = JsonConvert.SerializeObject(p);
            await _distributedCache.SetStringAsync($"product:{p.Id}", jsonProduct, options);

            // Byte olarak da tutulabilir, bunun için: (tavsiye json olarak kaydetme)
            //Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);
            //_distributedCache.Set("product:1", byteProduct);

            return View();
        }

        public async Task<IActionResult> Show()
        {
            //string name = _distributedCache.GetString("isim");
            //ViewBag.Name = name;
            Product product = JsonConvert.DeserializeObject<Product>(await _distributedCache.GetStringAsync("product:1"));
            ViewBag.Product = product;

            // Byte olarak kaydedileni alma
            //Byte[] byteProduct = _distributedCache.Get("product:1");
            //string jsonProduct = Encoding.UTF8.GetString(byteProduct);

            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("isim");

            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/image.jpg");
            Byte[] imageByte = System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("image", imageByte);

            return View();
        }

        public IActionResult ImageUrl()
        {
            Byte[] imageByte = _distributedCache.Get("image");
            return File(imageByte, "image/jpg");
        }

    }
}
