﻿using Search.Core.Entities;
using Search.Infrastructure;
using System;
using System.Net.Http;

namespace Search.IndexService
{
    public class Indexer
    {
        public Indexer(ElasticSearchClient client, ElasticSearchOptions options)
        {
            _client = client;
            _options = options;

            EnsureIndexCreated();
        }

        public void Index(IndexRequest request)
        {
            var document = request.Document;
            document.IndexedTime = DateTime.UtcNow;
            _client.Index(document, desc => desc
                .Id(document.Url.ToString())
                .Index(_options.DocumentsIndexName));
        }

        private readonly ElasticSearchClient _client;
        private readonly ElasticSearchOptions _options;

        private void EnsureIndexCreated()
        {
            var response = _client.IndexExists(_options.DocumentsIndexName);
            if (response.Exists)
                return;
            
            _client.CreateIndex(_options.DocumentsIndexName, index => index
                .Settings(ElasticSearchOptions.AnalysisSettings)
                .Mappings(mappings => mappings
                    .Map<DocumentInfo>(map => map
                        .Properties(properties => properties
                            .Text(ElasticSearchOptions.TitleProperty)
                            .Text(ElasticSearchOptions.TextProperty)
                        ).SourceField(source => source
                            .Excludes(new[] { "text" })
                        )
                    )
                )
            );
        }

        //получить html по данному url;
        public htmlDocument GetHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string result = content.ReadAsStringAsync().Result;
                    }
                }
            }
            return result;
        }
    }
}
