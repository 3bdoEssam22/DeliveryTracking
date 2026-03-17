using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Responses
{
    public class GenericResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
    }
}
