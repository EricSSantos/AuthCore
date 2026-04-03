using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de criação de usuário.</summary>
    public sealed class AddUser : IAddUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IConfirmCodePolicy _confirmCodePolicy;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailSender _emailSender;
        private readonly INotificationPolicy _notificationPolicy;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="confirmCodeRepository">Repositório de códigos de confirmação.</param>
        /// <param name="confirmCodePolicy">Política de códigos de confirmação.</param>
        /// <param name="passwordHasher">Serviço de hash de senhas.</param>
        /// <param name="emailSender">Serviço de envio de e-mails.</param>
        /// <param name="notificationPolicy">Política de notificações.</param>
        public AddUser(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IConfirmCodePolicy confirmCodePolicy,
            IPasswordHasher passwordHasher,
            IEmailSender emailSender,
            INotificationPolicy notificationPolicy)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _confirmCodePolicy = confirmCodePolicy;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
            _notificationPolicy = notificationPolicy;
        }

        /// <summary>Operação para criar o usuário com os dados informados.</summary>
        /// <param name="request">Dados do novo usuário.</param>
        public async Task OnExecuteAsync(AddUserRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new ConflictException("Não foi possível concluir o cadastro.");

            Password.ValidateWithConfirmation(request.Password, request.ConfirmPassword);
            var utcNow = DateTime.UtcNow;

            var user = User.Create(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                password: _passwordHasher.Hash(request.Password),
                utcNow: utcNow
            );

            await _userRepository.AddAsync(user);

            await SendNotification(user, utcNow);
        }

        #region Helpers

        /// <summary>Operação para enviar a notificação de confirmação de e-mail do usuário.</summary>
        /// <param name="user">Usuário recém-criado.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        private async Task SendNotification(User user, DateTime utcNow)
        {
            var codeValue = _confirmCodePolicy.GenerateCode(CodeType.ConfirmEmail, utcNow);
            var expiresAt = utcNow.Add(_confirmCodePolicy.GetExpiration(CodeType.ConfirmEmail));
            var payload = _notificationPolicy.CreatePayload(NotificationType.ConfirmEmail, codeValue);
            
            var email = EmailMessage.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: NotificationType.ConfirmEmail,
                utcNow: utcNow,
                payload: payload
            );

            var code = ConfirmCode.Create(
                code: codeValue,
                type: CodeType.ConfirmEmail,
                utcNow: utcNow,
                expiresAt: expiresAt
            );

            await _confirmCodeRepository.SetAsync(user.Id, code);
            await _emailSender.SendAsync(email);
        }

        #endregion
    }
}
