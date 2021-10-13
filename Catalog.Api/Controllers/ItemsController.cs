using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
   //[Route("items")] equivalent [Route("[controller]")]

   [ApiController]
   [Route("items")]
   public class ItemsController : ControllerBase
   {
       private readonly IItemsRepository repository;

       public ItemsController(IItemsRepository repository)
       {
           this.repository=repository;
       }

        //Get /items
       [HttpGet]
       public async Task<IEnumerable<ItemDto>> GetItemsAsync()
       {
           var items = (await repository.GetItemsAsync())
                       .Select(item => item.AsDto());
           return items;
       }

        //GET /item/{id}
       [HttpGet("{id}")]
       public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
       {
           var item = await repository.GetItemAsync(id);

           if(item is null)
           {
              return NotFound(); 
           }

           return Ok(item.AsDto()); 
       }

        //in convention we need to return the object created
       //POST /items
       [HttpPost]
       public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
       {
          Item item=new()
          {
             Id = Guid.NewGuid(),
             Name = itemDto.Name,
             Price = itemDto.Price,
             CreatedDate = DateTimeOffset.UtcNow
          };

         await repository.CreateItemAsync(item);

          return CreatedAtAction(nameof(GetItemAsync), new{id = item.Id}, item.AsDto()); 
       }

        //convention nothing to return
        //PUT /items/{id}
        [HttpPut("{id}")]
       public async Task<ActionResult> UpdateItemAsync(Guid id,UpdateItemDto itemDto)
       {
          var existingItem = await repository.GetItemAsync(id);

          if(existingItem is null)
          {
              return NotFound();
          }

          Item updatedItem = existingItem with
          {
              Name = itemDto.Name,
              Price=itemDto.Price
          };

          await repository.UpdateItemAsync(updatedItem);

          return NoContent();
       }


       //DELETE /items/{id}
       [HttpDelete("{id}")]
       public async Task<ActionResult> DeleteItemAsync(Guid id)
       {
          var existingItem = await repository.GetItemAsync(id);

          if(existingItem is null)
          {
              return NotFound();
          }

           await repository.DeleteItemAsync(id);

           return NoContent();       
       }

       
   } 
}