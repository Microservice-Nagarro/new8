using BHF.MS.new8.Controllers;
using BHF.MS.new8.Database.Models.DbItem;
using BHF.MS.new8.Database.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace BHF.MS.new8.Tests.Controllers
{
    public class DbItemsControllerTests
    {
        private readonly Mock<IDbItemService> _dbItemService = new();
        private readonly DbItemsController _sut;

        public DbItemsControllerTests()
        {
            _sut = new DbItemsController(_dbItemService.Object);
        }

        [Fact]
        public async Task GetDbItems_ReturnsOk()
        {
            // Arrange
            var list = new List<DbItemDto> { new() };
            _dbItemService.Setup(x => x.GetAll()).ReturnsAsync(list);

            // Act
            var result = (OkObjectResult)await _sut.GetDbItems();

            // Assert
            result.Value.Should().Be(list);
        }

        [Fact]
        public async Task GetDbItem_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _sut.GetDbItem(id);

            // Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetDbItem_WhenFound_ReturnsMatchingItem()
        {
            // Arrange
            var item = new DbItemDto { Id = Guid.NewGuid() };
            _dbItemService.Setup(x => x.GetById(item.Id)).ReturnsAsync(item);

            // Act
            var result = await _sut.GetDbItem(item.Id);

            // Assert
            result.Value.Should().Be(item);
        }

        [Fact]
        public async Task PutDbItem_WhenIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _sut.PutDbItem(id, new DbItemDto());

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutDbItem_WhenUpdateFails_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _sut.PutDbItem(id, new DbItemDto { Id = id });

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PutDbItem_WhenUpdateSucceeds_ReturnsNoContent()
        {
            // Arrange
            var model = new DbItemDto { Id = Guid.NewGuid() };
            _dbItemService.Setup(x => x.Update(model)).ReturnsAsync(true);

            // Act
            var result = await _sut.PutDbItem(model.Id, model);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task PostDbItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var model = new DbItemCreateDto();
            var output = new DbItemDto { Id = Guid.NewGuid() };
            _dbItemService.Setup(x => x.Add(model)).ReturnsAsync(output);

            // Act
            var result = await _sut.PostDbItem(model);

            // Assert
            result.ActionName.Should().Be(nameof(_sut.GetDbItem));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary(new { id = output.Id }));
            result.Value.Should().Be(output);
        }

        [Fact]
        public async Task DeleteDbItem_WhenDeleteFails_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _sut.DeleteDbItem(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteDbItem_WhenDeleteSucceeds_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _dbItemService.Setup(x => x.Delete(id)).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteDbItem(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

