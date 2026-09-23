using MovieApi.Contracts.Actors;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Actors
{
    internal static class ActorMapper
    {
        public static ActorResponse ToResponse(Actor actor)
        {
            return new ActorResponse(
                actor.Id,
                actor.FirstName,
                actor.LastName,
                actor.BirthDate,
                actor.CreatedAt,
                actor.UpdatedAt);
        }

        public static ActorShortResponse ToShortResponse(Actor actor)
        {
            return new ActorShortResponse(actor.Id, actor.FirstName, actor.LastName);
        }
    }
}
