using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class StringTypeController : BaseController
    {
        public StringTypeController(RedisService service) : base(service)
        {
        }

        public IActionResult Index()
        {
            _db.StringSet("name", "Erhan Baştürk");
            _db.StringSet("ziyaretci", 100);
            return View();
        }

        public IActionResult Show()
        {
            var value = _db.StringGet("name");
            //_db.StringIncrement("ziyaretci", 10);
            //var count = _db.StringDecrementAsync("ziyaretci", 1).Result; // azalt ve sonucu geri dön

            //_db.StringDecrementAsync("ziyaretci", 10).Wait(); // ben sonuçla ilgilenmiyorum sen sadece bu metodu çalıştır diyoruz

            if(value.HasValue)
            {
                ViewBag.name = value.ToString();
            }

            //value = _db.StringGetRange("name", 0, 3);
            //value = _db.StringLength("name");

            return View();
        }

    }
}
