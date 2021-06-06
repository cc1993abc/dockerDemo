using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using play.Catalog.Service.Dtos;
using System.Linq;
using Microsoft.Extensions.Logging;
using Play.Catalog.Service.Reoisitories;
using System.Threading.Tasks;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    // https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly ILogger<ItemsController> _logger;
        private readonly IItemsRepository _itemsRepository;

        /// <summary>
        /// 容器启动后的访问次数
        /// </summary>
        private static int num = 1;


        public ItemsController(ILogger<ItemsController> logger, IItemsRepository itemsRepository)
        {
            _logger = logger;
            _itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get()
        {
            var actionName = "GetItemsCount";
            await _itemsRepository.CountAsync(actionName);

            var items = (await _itemsRepository.GetAllAsync())
                .Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreatedItemDto createdItemDto)
        {
            var item = new Item
            {
                Name = createdItemDto.Name,
                Description = createdItemDto.Description,
                Price = createdItemDto.Price
            };
            await _itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existringItem = await _itemsRepository.GetAsync(id);

            if (existringItem == null)
            {
                return NotFound();
            }

            existringItem.Name = updateItemDto.Name;
            existringItem.Description = updateItemDto.Description;
            existringItem.Price = updateItemDto.Price;

            await _itemsRepository.UpdateAsync(existringItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existringItem = await _itemsRepository.GetAsync(id);

            if (existringItem == null)
            {
                return NotFound();
            }

            await _itemsRepository.RemoveAsync(id);

            return NoContent();
        }
    }
}