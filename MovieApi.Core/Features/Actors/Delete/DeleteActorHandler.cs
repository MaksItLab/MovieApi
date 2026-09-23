using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;

namespace MovieApi.Core.Features.Actors.Delete
{
    public sealed class DeleteActorHandler
    {
        private readonly IActorRepository _actorRepository;

        public DeleteActorHandler(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<OperationResult> HandleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetByIdAsync(id, cancellationToken);
            if (actor is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    $"Actor with id='{id}' was not found.");
            }

            if (await _actorRepository.HasMoviesAsync(id, cancellationToken))
            {
                return OperationResult.Failure(
                    OperationErrorType.Conflict,
                    "Actor is linked to movies and cannot be deleted.");
            }

            await _actorRepository.DeleteAsync(actor, cancellationToken);
            await _actorRepository.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
    }
}
