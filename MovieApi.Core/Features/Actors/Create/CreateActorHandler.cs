using MovieApi.Contracts.Actors;
using MovieApi.Core.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Actors.Create
{
    public sealed class CreateActorHandler
    {
        private readonly IActorRepository _actorRepository;

        public CreateActorHandler(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<OperationResult<ActorResponse>> HandleAsync(
            CreateActorRequest request,
            CancellationToken cancellationToken)
        {
            var validation = ActorValidation.Validate(request);
            if (!validation.IsSuccess)
            {
                return OperationResult<ActorResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var now = DateTime.UtcNow;

            var actor = new Actor
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                BirthDate = request.BirthDate?.Date,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _actorRepository.AddAsync(actor, cancellationToken);
            await _actorRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<ActorResponse>.Success(ActorMapper.ToResponse(actor));
        }
    }
}
