using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Http2ServerPush.Handler
{
    public class PushMiddleware
    {
        readonly RequestDelegate next;

        public PushMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() => {
                // read PushPromises from the HttpContext and convert them to Link headers.
                var promises = context.Items[PushPromiseHandler.HttpPushAttributeName] as IList<PushPromise>;
                if (promises != null)
                {
                    var header = string.Join(",", promises.Select(promise => promise.ToString()));
                    context.Response.Headers.Add("Link", header);
                }
                return Task.FromResult(0);
            });

            return next.Invoke(context);
        }

    }
}
