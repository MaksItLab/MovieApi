using MovieApi.Contracts.Actors;
using MovieApi.Core.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;

namespace MovieApi.Core.Features.Actors.Update
{
    public sealed class UpdateActorHandler
    {
        private readonly IActorRepository _actorRepository;

        public UpdateActorHandler(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<OperationResult<ActorResponse>> HandleAsync(
            Guid id,
            UpdateActorRequest request,
            CancellationToken cancellationToken)
        {
            var validation = ActorValidation.Validate(request);
            if (!validation.IsSuccess)
            {
                return OperationResult<ActorResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var actor = await _actorRepository.GetByIdAsync(id, cancellationToken);
            if (actor is null)
            {
                return OperationResult<ActorResponse>.Failure(
                    OperationErrorType.NotFound,
                    $"Actor with id='{id}' was not found.");
            }

            actor.FirstName = request.FirstName.Trim();
            actor.LastName = request.LastName.Trim();
            actor.BirthDate = request.BirthDate?.Date;
            actor.UpdatedAt = DateTime.UtcNow;

            await _actorRepository.UpdateAsync(actor, cancellationToken);
            await _actorRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<ActorResponse>.Success(ActorMapper.ToResponse(actor));
        }
    }
}
