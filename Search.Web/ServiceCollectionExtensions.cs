﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Search.Core.Elasticsearch;
using Search.Core.Entities;
using Search.IndexService;
using Search.SearchService;
using System;

namespace Search.Web
{
    public static class ServiceCollectionExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticSearchUrl = GetElasticSearchUrl(configuration);
            services.AddSingleton(new ElasticSearchOptions
            {
                Url = new Uri(elasticSearchUrl)
            });

            services.AddSingleton<ElasticSearchClient<Document>>();
        }

        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<Searcher>();
            services.AddSingleton<IRequestCache, MemoryRequestCache>();

            services.AddSingleton<Indexer>();
            services.AddSingleton<Reindexer>(provider => new Reindexer(
                provider.GetRequiredService<ElasticSearchClient<Document>>(),
                provider.GetRequiredService<ElasticSearchOptions>(),
                provider.GetRequiredService<Indexer>(),
                provider.GetRequiredService<QueueForIndex>()
               // autoReindexing: true,
                //indexingFrequency: TimeSpan.FromMinutes(1),
               // firstIndexingDeferral: TimeSpan.FromSeconds(10)
            ));

            services.AddSingleton<ServiceContainer>();
        }

        private static string GetElasticSearchUrl(IConfiguration configuration)
        {
            var url = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL");
            if (!string.IsNullOrEmpty(url))
                return url;

            url = configuration.GetValue<string>("ElasticSearchUrl");
            return url;
        }
    }
}
