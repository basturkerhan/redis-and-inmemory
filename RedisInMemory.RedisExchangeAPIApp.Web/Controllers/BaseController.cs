using Microsoft.AspNetCore.Mvc;
using RedisInMemory.RedisExchangeAPIApp.Web.Services;
using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly RedisService _service;
        protected readonly IDatabase _db;

        public BaseController(RedisService service)
        {
            _service = service;
            _db = service.GetDatabase(1);
        }

    }
}
