using System;
using System.Collections.Generic;
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
       private readonly InMemItemsRepository repository;

       public ItemsController()
       {
           repository=new InMemItemsRepository();
       }

        //Get /items
       [HttpGet]
       public IEnumerable<Item> GetItems()
       {
           var items = repository.GetItems();
           return items;
       }

        //Get /item/{id}
       [HttpGet("{id}")]
       public ActionResult<Item> GetItem(Guid id)
       {
           var item = repository.GetItem(id);
           if(item is null)
           {
              return NotFound(); 
           }

           return Ok(item); 
       }
   } 
}