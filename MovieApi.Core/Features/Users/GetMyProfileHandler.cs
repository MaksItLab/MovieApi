using MovieApi.Contracts.Users;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Users
{
    public sealed class GetMyProfileHandler
    {
        private const string CurrentUserUnavailableMessage =
            "Не удалось определить текущего пользователя.";

        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public GetMyProfileHandler(
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<OperationResult<UserProfileResponse>> HandleAsync(
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
            {
                return OperationResult<UserProfileResponse>.Failure(
                    OperationErrorType.Unauthorized,
                    CurrentUserUnavailableMessage);
            }

            var user = await _userRepository.GetByIdAsync(
                userId.Value,
                cancellationToken);

            if (user is null)
            {
                return OperationResult<UserProfileResponse>.Failure(
                    OperationErrorType.Unauthorized,
                    CurrentUserUnavailableMessage);
            }

            return OperationResult<UserProfileResponse>.Success(
                new UserProfileResponse(
                    user.Id,
                    user.Email,
                    user.CreatedAt));

        }
    }
}
