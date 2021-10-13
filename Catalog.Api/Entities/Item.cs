using System;

namespace Catalog.Api.Entities
{
    public class Item
    {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public decimal Price { get; set; }
      public string Description { get; set; }  
      public DateTimeOffset CreatedDate{ get; set; }

    }
}

//changement de type item to normal class
//for operation like update that can change the item