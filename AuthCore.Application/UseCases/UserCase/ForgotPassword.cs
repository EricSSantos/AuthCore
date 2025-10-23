using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate;

public sealed class ForgotPassword : IForgotPassword
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IConfirmCodeRepository _confirmCodeRepository;

    public ForgotPassword(
        IUserRepository userRepository,
        IEmailService emailService,
        IConfirmCodeRepository confirmCodeRepository)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _confirmCodeRepository = confirmCodeRepository;
    }

    public async Task OnExecute(ForgotPasswordInputModel input)
    {
        // Se o e-mail já estiver cadastrado, interrompe o processo silenciosamente.
        // Isso evita expor informações sobre contas existentes e protege contra
        // ataques de enumeração de e-mails válidos.
        var user = await _userRepository.GetByEmail(input.Email);
        if (user is null)
            return;

        var email = Email.Create(
            to: user.Email,
            fullName: user.FullName,
            type: EmailType.ForgotPassword
        );

        if (email.Payload is ForgotPasswordPayload payload)
        {
            var code = ConfirmCode.Create(
                payload.Code,
                CodeType.ForgotPassword
            );

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailService.Send(email);
        }
    }
}
