using MovieApi.Contracts.Actors;
using MovieApi.Core.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Actors.GetList
{
    public sealed class GetActorsHandler
    {
        private readonly IActorRepository _actorRepository;

        public GetActorsHandler(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<OperationResult<IReadOnlyList<ActorResponse>>> HandleAsync(
            CancellationToken cancellationToken)
        {
            var actors = await _actorRepository.GetAllAsync(cancellationToken);

            return OperationResult<IReadOnlyList<ActorResponse>>.Success(
                actors.Select(ActorMapper.ToResponse).ToList());
        }
    }
}
