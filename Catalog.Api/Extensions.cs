using Catalog.Api.Dtos;
using Catalog.Api.Entities;

namespace Catalog.Api
{

    //extension methode that concert item to itemDto
   public static class Extensions
   {
       public static ItemDto AsDto(this Item item)
       {
           return new ItemDto(item.Id,item.Name,item.Description,item.Price,item.CreatedDate);
       }
   }
}