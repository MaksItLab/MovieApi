using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Domain.Entities
{
    public sealed class Movie
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int ReleaseYear { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        #region Навигационные свойства
        public List<MovieGenre> MovieGenres { get; set; } = [];

        public List<MovieActor> MovieActors { get; set; } = [];

        public List<Review> Reviews { get; set; } = [];

        public MoviePoster? Poster { get; set; }

        public MovieVideo? Video { get; set; }
        #endregion
    }
}
