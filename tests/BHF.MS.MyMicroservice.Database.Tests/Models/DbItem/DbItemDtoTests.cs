using BHF.MS.new8.Database.Models.DbItem;
using FluentAssertions;

namespace BHF.MS.new8.Database.Tests.Models.DbItem
{
    public sealed class DbItemDtoTests
    {
        [Fact]
        public void Constructor_DbItem_ShouldCopyValues()
        {
            // Arrange
            var dbItem = new Context.Entities.DbItem
            {
                Name = "abc",
                Id = Guid.NewGuid()
            };
            var expectedResult = new DbItemDto
            {
                Id = dbItem.Id,
                Name = dbItem.Name
            };

            // Act
            var sut = new DbItemDto(dbItem);

            // Assert
            sut.Should().BeEquivalentTo(expectedResult);
        }
    }
}

