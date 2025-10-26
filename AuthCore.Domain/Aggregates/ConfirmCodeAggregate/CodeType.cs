using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Define os tipos de códigos de confirmação gerados pela aplicação.
    /// </summary>
    public enum CodeType
    {
        [Description("Código usado para redefinir a senha de um usuário.")]
        ForgotPassword
    }
}
