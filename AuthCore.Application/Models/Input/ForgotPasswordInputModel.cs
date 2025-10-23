using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthCore.Application.Models.Input
{
    public sealed record ForgotPasswordInputModel
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }
    }
}
