using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketTaggingRequestMarshal : IRequestMarshal<GetBucketTaggingRequest>
    {
        public Stream? MarshalRequest(GetBucketTaggingRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Tagging, string.Empty);
            return null;
        }
    }
}