using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Minio.Contracts
{
    public sealed record MinioClients(
        IMinioClient Internal,
        IMinioClient External);
}
