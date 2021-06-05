using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using play.Catalog.Service.Dtos;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Play.Catalog.Service.Controllers
{
    // https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of Hp", 5, System.DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "antidote", "Cures poison", 7, System.DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, System.DateTimeOffset.UtcNow)

        };
        private readonly ILogger<ItemsController> _logger;
        /// <summary>
        /// 容器启动后的访问次数
        /// </summary>
        private static int num = 1;


        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            _logger.LogInformation($"get static {num} ");
            num++;
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            return item != null ? item: NotFound();
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreatedItemDto createdItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createdItemDto.Name, createdItemDto.Description, createdItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existringItem = items.Where(item => item.Id == id).SingleOrDefault();

            var updatedItem = existringItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(item => item.Id == id);
            items[index] = updatedItem;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            items.RemoveAt(index);

            return NoContent();
        }
    }
}