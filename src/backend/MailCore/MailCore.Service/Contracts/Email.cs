using System.Text.Json;
using System.Text.Json.Serialization;

namespace MailCore.Service.Contracts
{
    /// <summary>Representa uma mensagem de e-mail consumida da fila.</summary>
    public sealed class Email
    {
        /// <summary>Identificador da mensagem de e-mail.</summary>
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        /// <summary>Endereço de destino do e-mail.</summary>
        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        /// <summary>Nome completo do destinatário.</summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        /// <summary>Tipo da notificação de e-mail.</summary>
        [JsonPropertyName("type")]
        public EmailType Type { get; init; }

        /// <summary>Versão do contrato serializado da mensagem.</summary>
        [JsonPropertyName("contract_version")]
        public int ContractVersion { get; init; }

        /// <summary>Payload dinâmico do tipo de notificação.</summary>
        [JsonPropertyName("payload")]
        public JsonElement? Payload { get; init; }

        /// <summary>Data de criação da mensagem em UTC.</summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        /// <summary>Valida os campos mínimos do contrato recebido.</summary>
        public void Validate()
        {
            if (TryValidate(out var errorMessage))
                return;
            throw new InvalidOperationException($"Mensagem de e-mail inválida: {errorMessage}");
        }

        /// <summary>Verifica se a mensagem possui os dados mínimos para processamento seguro.</summary>
        public bool TryValidate(out string errorMessage)
        {
            var errors = new List<string>();

            if (Id == Guid.Empty)
                errors.Add("O identificador da mensagem é obrigatório.");

            if (string.IsNullOrWhiteSpace(To))
                errors.Add("O destinatário da mensagem é obrigatório.");

            if (string.IsNullOrWhiteSpace(FullName))
                errors.Add("O nome do destinatário é obrigatório.");

            if (ContractVersion < 1)
                errors.Add("A versão do contrato da mensagem deve ser maior ou igual a 1.");

            if (CreatedAt == default)
                errors.Add("A data de criação da mensagem é obrigatória.");

            if (!Enum.IsDefined(typeof(EmailType), Type))
                errors.Add("O tipo de e-mail informado é inválido.");

            errorMessage = string.Join("; ", errors);
            return errors.Count == 0;
        }
    }
}
