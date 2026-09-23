using MovieApi.Contracts.Actors;
using MovieApi.Core.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Actors.GetById
{
    public sealed class GetActorByIdHandler
    {
        private readonly IActorRepository _actorRepository;

        public GetActorByIdHandler(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<OperationResult<ActorResponse>> HandleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetByIdAsync(id, cancellationToken);
            if (actor is null)
            {
                return OperationResult<ActorResponse>.Failure(
                    OperationErrorType.NotFound,
                    "Актёр не найден.");
            }

            return OperationResult<ActorResponse>.Success(ActorMapper.ToResponse(actor));
        }
    }
}
