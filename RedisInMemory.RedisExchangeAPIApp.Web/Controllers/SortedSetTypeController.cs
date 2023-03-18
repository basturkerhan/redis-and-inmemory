using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class SortedSetTypeController : BaseController
    {
        private string listKey = "sortedsetnames";

        public SortedSetTypeController(RedisService service) : base(service)
        {
        }

        public IActionResult Index()
        {
            HashSet<string> namesList = new HashSet<string>(); // normal listeden farkı içeride tuttuğu verileri sırasız olarak tutmasıdır
            if (_db.KeyExists(listKey))
            {
                _db.SortedSetScan(listKey).ToList().ForEach(x => namesList.Add(x.ToString()));
                //_db.SortedSetRangeByRank(listKey, order:Order.Descending).ToList().ForEach(x => namesList.Add(x.ToString()));
            }
            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            _db.SortedSetAdd(listKey, name, score);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.SortedSetRemoveAsync(listKey, name);
            return RedirectToAction("Index");
        }

    }
}
