using System;
using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration.</summary>
    public class PutBucketLifecycleConfigurationRequest : BaseRequest, IHasBucketName, IContentMd5Config
    {
        public PutBucketLifecycleConfigurationRequest(string bucketName, IEnumerable<S3Rule> rules) : base(HttpMethod.PUT)
        {
            Initialize(bucketName, rules);
        }

        internal void Initialize(string bucketName, IEnumerable<S3Rule> rules)
        {
            BucketName = bucketName;
            Rules = rules.ToList();
        }

        public string BucketName { get; set; }
        public IList<S3Rule> Rules { get; private set; }
        public Func<bool> ForceContentMd5 => () => true;
        public byte[]? ContentMd5 { get; set; }

        public override void Reset()
        {
            ContentMd5 = null;

            base.Reset();
        }
    }
}