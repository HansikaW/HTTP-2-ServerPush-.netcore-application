using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Http2ServerPush.Handler
{
    public class PushPromise
    {
        public PushPromise(string url, string assetType)
        {
            Url = url;
            AssetType = assetType;
        }

        public string AssetType { get; }
        public string Url { get; }

        public override string ToString()
        {
            return $"<{Url}>; rel=preload; as={AssetType}";
        }
    }
}
