using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Movies
{
    public sealed record UploadedFile(
        string OriginalFileName,
        long Length,
        Func<Stream> OpenReadStream);
}
