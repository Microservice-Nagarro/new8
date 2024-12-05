using BHF.MS.new8.Database.Models.DbItem;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BHF.MS.new8.Database.Context.Entities
{
    [ExcludeFromCodeCoverage(Justification = "It's EF entity")]
    public class DbItem
    {
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

        public void Update(DbItemDto dto)
        {
            Name = dto.Name;
            LastUpdated = DateTime.UtcNow;
        }
    }
}

