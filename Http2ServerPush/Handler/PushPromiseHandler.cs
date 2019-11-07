using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Http2ServerPush.Handler
{

    [HtmlTargetElement("script", Attributes = HttpPushAttributeName)]
    [HtmlTargetElement("link", Attributes = HttpPushAttributeName)]
    [HtmlTargetElement("img", Attributes = HttpPushAttributeName)]
    [HtmlTargetElement("a", Attributes = HttpPushAttributeName)]
    public class PushPromiseHandler : TagHelper
    {
        internal const string HttpPushAttributeName = "http-push";
        readonly IHttpContextAccessor contextAccessor;
        public PushPromiseHandler(IHttpContextAccessor contextAccessor) { this.contextAccessor = contextAccessor; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // get the resource to push from the HTML tag.
            var resource = ReadPushedResourceFromTag(output.TagName, context.AllAttributes);

            // store the resource in our HttpContext (per-request). It will be used in HttpPushMiddleware.
            StoreResourceInContext(resource, contextAccessor.HttpContext);

            // remove http-push attribute from HTML output
            output.Attributes.RemoveAll(HttpPushAttributeName);
        }

        /// <summary>
        /// Given an HTML tag (with attributes), read the resource that should be pushed to the client.
        /// </summary>
        private PushPromise ReadPushedResourceFromTag(string tagName, ReadOnlyTagHelperAttributeList attributes)
        {
            switch (tagName)
            {
                case "script":
                    // create Link: </asset/to/push.js>; rel=preload; as=script
                    return new PushPromise(attributes["src"].Value.ToString(), "script");
                case "link":
                    // create Link: </asset/to/push.css>; rel=preload; as=stylesheet
                    return new PushPromise(attributes["href"].Value.ToString(), attributes["rel"].Value?.ToString());
                case "img":
                    // create Link: </asset/to/push.jpg>; rel=preload; as=image
                    return new PushPromise(attributes["src"].Value.ToString(), "image");
                case "a":
                    // create Link: </asset/to/push.html>; rel=preload; as=document
                    return new PushPromise(attributes["href"].Value.ToString(), "document");
                default:
                    throw new NotImplementedException($"{nameof(Handler.PushPromiseHandler)} does not know how to handle '{tagName}' tags");
            }
        }

        /// <summary>
        /// Store the PushPromise object in the HttpContext in a per-request store.
        /// </summary>
        private void StoreResourceInContext(PushPromise resource, HttpContext httpContext)
        {
            var pushedResources = httpContext.Items[HttpPushAttributeName] as List<PushPromise> ?? new List<PushPromise>();
            pushedResources.Add(resource);
            httpContext.Items[HttpPushAttributeName] = pushedResources;
        }

    }

}
 