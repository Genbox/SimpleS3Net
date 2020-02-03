﻿using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Tests.Code.Other;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests.Objects
{
    public class NonSeekableStreamTests : OfflineTestBase
    {
        public NonSeekableStreamTests(ITestOutputHelper outputHelper) : base(outputHelper, CustomConfig)
        {
        }

        private static void CustomConfig(S3Config obj)
        {
            //We force streaming signatures as it is the only one that supports non-seekable streams
            obj.StreamingChunkSize = 8096;
            obj.PayloadSignatureMode = SignatureMode.StreamingSignature;
        }

        [Fact]
        public async Task SendNonSeekableStream()
        {
            byte[] data = new byte[1024 * 10];
            Array.Fill(data, (byte)'A');

            //We test if it is possible send a non-seekable stream. This should succeed as we use ChunkedStream
            PutObjectResponse resp = await ObjectClient.PutObjectAsync(BucketName, nameof(SendNonSeekableStream), new NonSeekableStream(data)).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);
        }
    }
}