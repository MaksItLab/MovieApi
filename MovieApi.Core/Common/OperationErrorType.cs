using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Common
{
    public enum OperationErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Unavailable
    }
}
