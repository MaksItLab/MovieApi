using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Domain.Entities
{
    public sealed class MoviePoster
    {
        public Guid MovieId { get; set; }

        public string ObjectKey { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public DateTime UploadedAt { get; set; }

        #region Навигационные свойства

            public Movie Movie { get; set; } = null!;
        #endregion
    }
}
