using AuthCore.Domain.Entities;
using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Output
{
    public sealed record UserViewModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        public static UserViewModel ToViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FirstName + user.LastName,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
