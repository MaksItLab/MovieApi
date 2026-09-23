using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Domain.Entities
{
    public sealed class Review
    {
        public Guid Id { get; set; }

        public Guid MovieId { get; set; }

        public Guid AuthorId { get; set; }

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        #region Навигационные свойства
        public Movie Movie { get; set; } = default!;

        public User Author { get; set; } = default!;
        #endregion
    }
}
