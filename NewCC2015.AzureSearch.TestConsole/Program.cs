using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using NewCC2015.AzureSearch.Core;

namespace NewCC2015.AzureSearch.TestConsole
{
    class Program
    {
        private static readonly string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        private static readonly string searchIndex = ConfigurationManager.AppSettings["SearchIndexName"];
        private static readonly string searchApiKey = ConfigurationManager.AppSettings["SearchApiKey"];
        private static readonly SearchCredentials searchCredentials = new SearchCredentials(searchApiKey);
        private static readonly SearchServiceClient searchClient = new SearchServiceClient(searchServiceName, searchCredentials);

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var indexClient = searchClient.Indexes.GetClient(searchIndex);

                var sp = new SearchParameters();
                sp.Facets = new[] { "source", "retweets", "following", "followers,values:1000|5000|10000|50000|100000" };

                var response = await indexClient.Documents.SearchAsync<MarchMadnessTweet>("in", sp);
                

                foreach (var searchResult in response.Results)
                {
                    Console.WriteLine(searchResult.Document);
                    Console.WriteLine("Score:{0}", searchResult.Score);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Console.ReadLine();

            }).Wait();
        }
    }
}
