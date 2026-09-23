using MovieApi.Contracts.Auth;
using MovieApi.Core.Auth;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Security;
using MovieApi.Core.Users;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Auth
{
    public sealed class RegisterHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;

        public RegisterHandler(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task<OperationResult<AuthUserResponse>> HandleAsync(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var emailValidation = UserValidation.ValidateEmail(request.Email);
            if (!emailValidation.IsSuccess)
            {
                return OperationResult<AuthUserResponse>.Failure(
                    emailValidation.ErrorType!.Value,
                    emailValidation.ErrorMessage!);
            }

            var passwordValidation = UserValidation.ValidatePassword(request.Password);
            if (!passwordValidation.IsSuccess)
            {
                return OperationResult<AuthUserResponse>.Failure(
                    passwordValidation.ErrorType!.Value,
                    passwordValidation.ErrorMessage!);
            }

            var email = request.Email.Trim();
            var normalizedEmail = UserValidation.NormalizeEmail(request.Email);

            if (await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
            {
                return OperationResult<AuthUserResponse>.Failure(
                    OperationErrorType.Conflict,
                    "Пользователь с таким email уже существует.");
            }

            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                NormalizedEmail = normalizedEmail,
                CreatedAt = now,
                UpdatedAt = now
            };

            user.PasswordHash = _passwordHashService.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<AuthUserResponse>.Success(AuthMapper.ToResponse(user));
        }
    }
}
