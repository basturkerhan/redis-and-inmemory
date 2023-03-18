using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class HashTypeController : BaseController
    {
        private string hashKey = "sozluk";

        public HashTypeController(RedisService service) : base(service)
        {
        }

        public IActionResult Index()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            if(_db.KeyExists(hashKey))
            {
                _db.HashGetAll(hashKey).ToList().ForEach(x => 
                    list.Add(x.Name.ToString(), x.Value.ToString())
                );
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string name, string val)
        {
            _db.HashSet(hashKey, name, val);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string name)
        {
            _db.HashDelete(hashKey, name);
            return RedirectToAction("Index");
        }

    }
}
