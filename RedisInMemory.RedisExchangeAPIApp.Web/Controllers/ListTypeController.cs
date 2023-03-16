﻿using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _service;
        private readonly IDatabase _db;
        private string listKey = "names";

        public ListTypeController(RedisService service)
        {
            _service = service;
            _db = service.GetDatabase(1);
        }

        public IActionResult Index()
        {
            List<string> namesList = new List<string>();
            if(_db.KeyExists(listKey))
            {
                _db.ListRange(listKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });
            }

            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            _db.ListRightPush(listKey, name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.ListRemoveAsync(listKey, name);
            //_db.ListLeftPopAsync(listKey); // listenin başından siler
            return RedirectToAction("Index");
        }

    }
}
