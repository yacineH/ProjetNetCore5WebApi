using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Entities;

namespace Catalog.Api.Repositories
{   
    //2 changes for Async :
    //all the methods should return a task
    //Convention :the name with finnish with Async 
    public interface IItemsRepository
    {
        Task<Item> GetItemAsync(Guid id);
        
        Task<IEnumerable<Item>> GetItemsAsync();
        
        Task CreateItemAsync(Item item);

        Task UpdateItemAsync(Item item);

        Task DeleteItemAsync(Guid id);
    }
}