using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BHF.MS.new8.Models.Settings
{
    [ExcludeFromCodeCoverage(Justification = "It's a model with no logic")]
    public class ServiceSettings
    {
        [Required(ErrorMessage = "Name required")]
        public string Name { get; set; } = string.Empty;
        public string Endpoint1Uri { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public HttpClientSettings HttpClient { get; set; } = new();
    }
}

