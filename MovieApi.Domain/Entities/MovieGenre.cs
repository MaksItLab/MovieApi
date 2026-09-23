using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Domain.Entities
{
    public class MovieGenre
    {
        public Guid MovieId { get; set; }
        public Guid GenreId { get; set; }

        #region Навигационные свойства
        public Movie Movie { get; set; } = default!;
        public Genre Genre { get; set; } = default!;
        #endregion
    }
}
