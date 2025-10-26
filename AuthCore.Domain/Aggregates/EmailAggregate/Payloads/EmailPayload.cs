using AuthCore.Domain.Core.Interfaces;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Payloads
{
    /// <summary>
    /// Classe base para representar dados adicionais transportados por um e-mail.
    /// Cada tipo de e-mail pode definir seu próprio payload específico.
    /// </summary>
    public abstract class EmailPayload
    { }
}
