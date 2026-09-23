using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Reviews
{
    public sealed record UpdateReviewRequest(
        string Text,
        int Rating);
}
