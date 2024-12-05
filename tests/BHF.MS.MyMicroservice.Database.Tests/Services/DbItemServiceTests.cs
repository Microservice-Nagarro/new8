using BHF.MS.new8.Database.Context;
using BHF.MS.new8.Database.Context.Entities;
using BHF.MS.new8.Database.Models.DbItem;
using BHF.MS.new8.Database.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System.Globalization;

namespace BHF.MS.new8.Database.Tests.Services
{
    public sealed class DbItemServiceTests
    {
        private readonly DbItemService _sut;
        private readonly Mock<CustomDbContext> _contextMock = new(new DbContextOptions<CustomDbContext>());

        public DbItemServiceTests()
        {
            _sut = new DbItemService(_contextMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsTop100OrderedByLastUpdated()
        {
            // Arrange
            var entityList = new List<DbItem>();
            for (var i = 0; i < 101; i++)
            {
                entityList.Add(new DbItem
                {
                    Id = Guid.NewGuid(),
                    Name = i.ToString(CultureInfo.InvariantCulture)
                });
            }

            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet(entityList);

            // Act
            var result = await _sut.GetAll();

            // Assert
            result.Should().HaveCountLessOrEqualTo(100);

            var minLastUpdated = entityList.MinBy(x => x.LastUpdated);
            result.Should().NotContain(x => x.Id == minLastUpdated!.Id);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([]);

            // Act
            var result = await _sut.GetById(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsItem()
        {
            // Arrange
            var dbItem = new DbItem { Id = Guid.NewGuid() };
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([dbItem]);

            // Act
            var result = await _sut.GetById(dbItem.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(dbItem.Id);
        }

        [Fact]
        public async Task Update_WhenNotFound_ReturnsFalse_StopsFurtherExecution()
        {
            // Arrange
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([]);

            // Act
            var result = await _sut.Update(new DbItemDto { Id = Guid.NewGuid() });

            // Assert
            result.Should().BeFalse();
            _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Update_WhenDbUpdateConcurrencyException_ReturnsFalse(bool itemFound)
        {
            // Arrange
            var dbItem = new DbItem { Id = Guid.NewGuid() };
            var model = new DbItemDto(dbItem);

            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet(itemFound ? [dbItem] : []);
            _contextMock.Setup(x => x.DbItems.FindAsync(dbItem.Id))
                .ReturnsAsync(dbItem);

            _contextMock.Setup(x => x.SaveChangesAsync(default)).Throws<DbUpdateConcurrencyException>();

            // Act
            var result = await _sut.Update(model);

            // Assert
            result.Should().Be(itemFound);
        }

        [Fact]
        public async Task Update_WhenRandomExceptionThrown_ShouldNotCatchException()
        {
            // Arrange
            var dbItem = new DbItem { Id = Guid.NewGuid() };
            var model = new DbItemDto(dbItem);
            _contextMock.Setup(x => x.DbItems.FindAsync(dbItem.Id))
                .ReturnsAsync(dbItem);
            _contextMock.Setup(x => x.SaveChangesAsync(default)).Throws<Exception>();

            // Act
            var act = () => _sut.Update(model);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Update_WhenNoException_ShouldReturnTrue()
        {
            // Arrange
            var dbItem = new DbItem { Id = Guid.NewGuid() };
            var model = new DbItemDto(dbItem) { Name = "abc" };
            _contextMock.Setup(x => x.DbItems.FindAsync(dbItem.Id))
                .ReturnsAsync(dbItem);

            // Act
            var result = await _sut.Update(model);

            // Assert
            result.Should().BeTrue();
            dbItem.Name.Should().Be(model.Name);
        }

        [Fact]
        public async Task Add_ReturnsNewlyCreatedDto()
        {
            // Arrange
            var createModel = new DbItemCreateDto { Name = "abc" };
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([]);
            var expectedOutput = new DbItemDto { Name = createModel.Name };

            // Act
            var result = await _sut.Add(createModel);

            // Assert
            result.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public async Task Delete_WhenItemNotFound_ReturnsFalse_StopsFurtherExecution()
        {
            // Arrange
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([]);

            // Act
            var result = await _sut.Delete(Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
            _contextMock.Verify(x => x.DbItems.Remove(It.IsAny<DbItem>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenItemFound_ReturnsTrue()
        {
            // Arrange
            var item = new DbItem { Id = Guid.NewGuid() };
            _contextMock.SetupGet(x => x.DbItems).ReturnsDbSet([]);
            _contextMock.Setup(x => x.DbItems.FindAsync(item.Id))
                .ReturnsAsync(item);

            // Act
            var result = await _sut.Delete(item.Id);

            // Assert
            result.Should().BeTrue();
        }
    }
}

