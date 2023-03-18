using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class SetTypeController : BaseController
    {
        private string listKey = "setnames";

        public SetTypeController(RedisService service) : base(service)
        {
        }

        public IActionResult Index()
        {
            HashSet<string> namesList = new HashSet<string>(); // normal listeden farkı içeride tuttuğu verileri sırasız olarak tutmasıdır
            if(_db.KeyExists(listKey))
            {
                _db.SetMembers(listKey).ToList().ForEach(name => namesList.Add(name.ToString()));
            }
            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            // eğer absolute expiration eklemek istersek:
            if(!_db.KeyExists(listKey))
            {
                // eğer yoksa ekle diyoruz bu sebeple bu süre sadece 1 kere ekleniyor
                _db.KeyExpire(listKey, DateTime.Now.AddMinutes(20));
            }
            _db.KeyExpire(listKey, DateTime.Now.AddMinutes(5)); // normalde Exchange API'da sliding expiration yok ama bunu her çalıştığında 5 dakika ekle diyerek ona benzer bir kullanım yapabiliriz
            _db.SetAdd(listKey, name);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.SetRemoveAsync(listKey, name);
            return RedirectToAction("Index");
        }

    }
}
