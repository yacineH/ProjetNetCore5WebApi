using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
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
       public IEnumerable<ItemDto> GetItems()
       {
           var items = repository.GetItems().Select(item => item.AsDto());
           return items;
       }

        //Get /item/{id}
       [HttpGet("{id}")]
       public ActionResult<ItemDto> GetItem(Guid id)
       {
           var item = repository.GetItem(id);

           if(item is null)
           {
              return NotFound(); 
           }

           return Ok(item.AsDto()); 
       }
   } 
}