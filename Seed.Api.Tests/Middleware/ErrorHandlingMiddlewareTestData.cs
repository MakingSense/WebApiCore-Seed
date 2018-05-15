using System;
using System.Collections;
using System.Collections.Generic;
using WebApiCoreSeed.Domain.Exceptions;

namespace WebApiCoreSeed.WebApi.Tests.Middleware
{
    public class ErrorHandlingMiddlewareTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new Exception("Message"), 500 };
            yield return new object[] { new InvalidCastException("Message"), 500 };
            yield return new object[] { new UnauthorizedException("Message"), 401 };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
