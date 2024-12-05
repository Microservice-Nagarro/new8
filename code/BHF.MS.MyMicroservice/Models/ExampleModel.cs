using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BHF.MS.new8.Models
{
    [ExcludeFromCodeCoverage(Justification = "It's a model with no logic")]
    public class ExampleModel
    {
        [JsonRequired]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters")]
        public required string Title { get; set; }

        [Range(0, 999.99)]
        public decimal Price { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        public required string Phone { get; set; }

        public Dictionary<string, string>? Customisations { get; set; }
    }
}

