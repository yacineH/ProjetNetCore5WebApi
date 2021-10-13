using System;
using System.Threading.Tasks;
using Catalog.Api.Repositories;
using Catalog.Api.Entities;
using Microsoft.Extensions.Logging;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();
        private readonly Random rand = new();


        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnNotFound()
        {
            //Arrange           
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
               .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task GetItemAsync_WithExisttingItem_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = CreateRandomItem();
           
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object,loggerStub.Object);
           
            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            //Assert          
            result.Value.Should().BeEquivalentTo(
               expectedItem,
               options => options.ComparingByMembers<Item>());
        }

        [Fact]
        public async Task GetItemsAsync_WithItemToCreate_ReturnsTheCreatedItem()
        {
           //Arrange
            var expectedItems=new[]
            {
              CreateRandomItem(),
              CreateRandomItem(),
              CreateRandomItem()
            }; 
            
            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);
            
            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);

           //Act
           var actualItems= await controller.GetItemsAsync();

           //Assert
           actualItems.Should().BeEquivalentTo(
             expectedItems,
             options => options.ComparingByMembers<Item>());
           
        }

        [Fact]
        public async Task CreateItemAsync__()
        {
          //Arrange            
            var itemToCreate = new CreateItemDto()
            {
                Name=Guid.NewGuid().ToString(),
                Price=rand.Next(1000)
            };

            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
          //Act
            var result =await controller.CreateItemAsync(itemToCreate);
          //Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1000));
        }

        [Fact]
        public async Task UpdateItemAsync_WithUnexistingItem_ReturnsNotFoundExceptio()
        {
            //Arrange
            var itemToUpdate=new UpdateItemDto()
            {
                Name=Guid.NewGuid().ToString(),
                Price=rand.Next(1000)
            };

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Item)null);

            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result=await controller.UpdateItemAsync(Guid.NewGuid(),itemToUpdate);

            //Assert
             result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange

            Item existingItem=CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                     .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate=new UpdateItemDto()
            {
                Name=Guid.NewGuid().ToString(),
                Price=existingItem.Price + 3
            };
            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
            var result=await controller.UpdateItemAsync(itemId,itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

         [Fact]
         public async Task DeleteItemAsync_WithUnExistingItem_ReturnsNotFound()
         {
            //Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Item)null);
             var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
             var result= await controller.DeleteItemAsync(Guid.NewGuid());
            //Assert
            result.Should().BeOfType<NotFoundResult>();
         }

         [Fact]
         public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
         {
            //Arrange
            var existingItem=CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);
             var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            //Act
             var result= await controller.DeleteItemAsync(existingItem.Id);
            //Assert
            result.Should().BeOfType<NotFoundResult>();
         }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
