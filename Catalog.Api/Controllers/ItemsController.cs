using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
   //[Route("items")] equivalent [Route("[controller]")]

   [ApiController]
   [Route("items")]
   public class ItemsController : ControllerBase
   {
       private readonly IItemsRepository repository;

       private readonly ILogger<ItemsController> logger;

       public ItemsController(IItemsRepository repository,ILogger<ItemsController> logger)
       {
           this.repository=repository;
           this.logger=logger;
       }

        //Get /items
       [HttpGet]
       public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name=null)
       {
           var items = (await repository.GetItemsAsync())
                       .Select(item => item.AsDto());

            if(!string.IsNullOrWhiteSpace(name))
            {
              items=items.Where(items=>items.Name.Contains(name,StringComparison.OrdinalIgnoreCase));
            }  
           
           logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}:Retrieved {items.Count()} items");
          
           return items;
       }


        //on rajoute le parametre dans la methode avce la meme
        //signature et on le met optionel nameTomatch=null 
        // public Task<IEnumerable<ItemDto>> GetItemsAsync()
        // {
        //     throw new NotImplementedException();
        // }


        //GET /item/{id}
       [HttpGet("{id}")]
       public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
       {
           var item = await repository.GetItemAsync(id);

           if(item is null)
           {
              return NotFound(); 
           }

           return item.AsDto(); 
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
             Description=itemDto.Description,
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

          existingItem.Name = itemDto.Name;
          existingItem.Price=itemDto.Price;
          
          await repository.UpdateItemAsync(existingItem);

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