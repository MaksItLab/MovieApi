using MovieApi.Contracts.Reviews;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Reviews
{
    internal static class ReviewMapper
    {
        public static ReviewResponse ToResponse(Review review)
        {
            return new ReviewResponse(
                review.Id,
                review.MovieId,
                review.AuthorId,
                review.Text,
                review.Rating,
                review.CreatedAt,
                review.UpdatedAt);
        }
    }
}
