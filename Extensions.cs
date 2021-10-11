using Catalog.Dtos;
using Catalog.Entities;

namespace Catalog
{

    //extension methode that concert item to itemDto
   public static class Extensions
   {
       public static ItemDto AsDto(this Item item)
       {
           return new ItemDto{
               Id = item.Id,
               Name = item.Name,
               Price = item.Price,
               CreatedDate = item.CreatedDate

           };
       }
   }
}