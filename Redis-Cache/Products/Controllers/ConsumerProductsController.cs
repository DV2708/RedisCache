using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Products.Context;
using System.Text;
using System.Text.Json;

namespace Products.Model.Product.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public ConsumerProductsController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cacheKey = "GET_ALL_PRODUCTS";
            List<ConsumerProduct> consumerProducts;

            // Get data from cache
            var cachedData = await _cache.GetAsync(cacheKey);
            if (cachedData != null)
            {
                // If data found in cache, encode and deserialize cached data
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                consumerProducts = JsonSerializer.Deserialize<List<ConsumerProduct>>(cachedDataString) ?? new List<ConsumerProduct>();
            }
            else
            {
                // If not found, then fetch data from database
                consumerProducts = await _context.Products.ToListAsync();

                // serialize data
                var cachedDataString = JsonSerializer.Serialize(consumerProducts);
                var newDataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                // set cache options 
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                // Add data in cache
                await _cache.SetAsync(cacheKey, newDataToCache, options);
            }

            return Ok(consumerProducts);
        }
    }
}