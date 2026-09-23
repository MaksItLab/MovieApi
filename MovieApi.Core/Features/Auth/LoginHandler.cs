using MovieApi.Contracts.Auth;
using MovieApi.Core.Auth;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Security;
using MovieApi.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Auth
{
    public sealed class LoginHandler
    {
        private const string InvalidCredentialsMessage = "Email или пароль указаны неверно.";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IAccessTokenService _accessTokenService;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IAccessTokenService accessTokenService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _accessTokenService = accessTokenService;
        }

        public async Task<OperationResult<LoginResponse>> HandleAsync(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var emailValidation = UserValidation.ValidateEmail(request.Email);
            if (!emailValidation.IsSuccess)
            {
                return OperationResult<LoginResponse>.Failure(
                    emailValidation.ErrorType!.Value,
                    emailValidation.ErrorMessage!);
            }

            var passwordValidation = UserValidation.ValidatePassword(request.Password);
            if (!passwordValidation.IsSuccess)
            {
                return OperationResult<LoginResponse>.Failure(
                    passwordValidation.ErrorType!.Value,
                    passwordValidation.ErrorMessage!);
            }

            var normalizedEmail = UserValidation.NormalizeEmail(request.Email);
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null)
            {
                return OperationResult<LoginResponse>.Failure(
                    OperationErrorType.Unauthorized,
                    InvalidCredentialsMessage);
            }

            var isPasswordValid = _passwordHashService.VerifyPassword(user, request.Password);
            if (!isPasswordValid)
            {
                return OperationResult<LoginResponse>.Failure(
                    OperationErrorType.Unauthorized,
                    InvalidCredentialsMessage);
            }

            var accessToken = _accessTokenService.Create(user);

            return OperationResult<LoginResponse>
                .Success(new LoginResponse
                (
                    accessToken.Value,
                    "Bearer",
                    accessToken.ExpiresAtUtc,
                    AuthMapper.ToResponse(user)
                ));
        }
    }
}
