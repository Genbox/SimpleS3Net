﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Aws;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Internals;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Internals.Validation;
using Genbox.SimpleS3.Core.Operations;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IValidatorFactory = Genbox.SimpleS3.Core.Abstracts.Factories.IValidatorFactory;

namespace Genbox.SimpleS3.Core.Extensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<AwsConfig, IServiceProvider> config)
        {
            collection.Configure(config);
            return AddSimpleS3Core(collection);
        }

        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<AwsConfig> config)
        {
            collection.Configure(config);
            return AddSimpleS3Core(collection);
        }

        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection)
        {
            collection.AddLogging();
            collection.AddOptions();

            //Authentication
            collection.AddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
            collection.AddSingleton<IScopeBuilder, ScopeBuilder>();
            collection.AddSingleton<ISignatureBuilder, SignatureBuilder>();
            collection.AddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
            collection.AddSingleton<HeaderAuthorizationBuilder>();
            collection.AddSingleton<ISignedRequestHandler, DefaultSignedRequestHandler>();
            collection.AddSingleton<QueryParameterAuthorizationBuilder>();

            //Operations
            collection.AddSingleton<IObjectOperations, ObjectOperations>();
            collection.AddSingleton<IBucketOperations, BucketOperations>();
            collection.AddSingleton<IMultipartOperations, MultipartOperations>();
            collection.AddSingleton<ISignedObjectOperations, SignedObjectOperations>();

            //Clients
            collection.AddSingleton<IObjectClient, S3ObjectClient>();
            collection.AddSingleton<IBucketClient, S3BucketClient>();
            collection.AddSingleton<IMultipartClient, S3MultipartClient>();
            collection.AddSingleton<ISignedObjectClient, S3SignedObjectClient>();

            //Misc
            collection.AddSingleton<IRequestHandler, DefaultRequestHandler>();
            collection.AddSingleton<IValidatorFactory, ValidatorFactory>();
            collection.AddSingleton<IMarshalFactory, MarshalFactory>();
            collection.AddSingleton<IPostMapperFactory, PostMapperFactory>();
            collection.AddSingleton<IRequestStreamWrapper, ChunkedContentRequestStreamWrapper>();

            //Default services for AWS S3
            collection.AddSingleton<IUrlBuilder, AwsUrlBuilder>();
            collection.AddSingleton<IInputValidator, AwsInputValidator>();
            collection.AddSingleton<IRegionData, AwsRegionData>();

            //Fluent
            collection.AddSingleton<ITransfer, Transfer>();
            collection.AddSingleton<IMultipartTransfer, MultipartTransfer>();

            Assembly assembly = typeof(AwsConfig).Assembly; //Needs to be the assembly that contains the types

            collection.TryAddEnumerable(CreateRegistrations(typeof(IInputValidator), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IRequestMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IResponseMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IPostMapper), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IValidator), assembly));

            return new CoreBuilder(collection);
        }

        private static IEnumerable<ServiceDescriptor> CreateRegistrations(Type abstractType, Assembly assembly)
        {
            foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            {
                yield return ServiceDescriptor.Singleton(abstractType, type);
            }
        }
    }
}