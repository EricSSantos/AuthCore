using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.ConfirmCodes
{
    /// <summary>Define os tipos de códigos de confirmação.</summary>
    public enum CodeType
    {
        ConfirmEmail,
        ForgotPassword
    }
}
