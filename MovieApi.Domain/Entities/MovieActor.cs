using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Domain.Entities
{
    public sealed class MovieActor
    {
        public Guid MovieId { get; set; }
        public Guid ActorId { get; set; }

        #region Навигационные свойства
        public Movie Movie { get; set; } = default!;
        public Actor Actor { get; set; } = default!;
        #endregion
    }
}
