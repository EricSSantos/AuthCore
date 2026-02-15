using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Users.Contracts;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Contracts;
using AuthCore.Domain.Aggregates.ConfirmCodes.Policies;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Contracts;
using AuthCore.Domain.Aggregates.Notifications.Policies;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Contracts;
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

        public async Task OnExecuteAsync(AddUserRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new ConflictException("E-mail já cadastrado.");

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

        private async Task SendNotification(User user, DateTime utcNow)
        {
            var codeValue = _confirmCodePolicy.GenerateCode(CodeType.ConfirmEmail, utcNow);
            var expiresAt = utcNow.Add(_confirmCodePolicy.GetExpiration(CodeType.ConfirmEmail));
            var email = Notification.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: NotificationType.ConfirmEmail,
                utcNow: utcNow,
                code: codeValue,
                notificationPolicy: _notificationPolicy
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
