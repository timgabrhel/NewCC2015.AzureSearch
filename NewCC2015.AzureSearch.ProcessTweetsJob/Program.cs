using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using NewCC2015.AzureSearch.Core;

namespace NewCC2015.AzureSearch.ProcessTweetsJob
{
    public class Program
    {
        private static readonly string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        private static readonly string searchIndex = ConfigurationManager.AppSettings["SearchIndexName"];
        private static readonly string searchApiKey = ConfigurationManager.AppSettings["SearchApiKey"];
        private static readonly SearchCredentials searchCredentials = new SearchCredentials(searchApiKey);
        private static readonly SearchServiceClient searchClient = new SearchServiceClient(searchServiceName, searchCredentials);

        public static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static async void ProcessTweet([QueueTrigger(Strings.MarchMadnessQueueName)] MarchMadnessTweet tweet)
        {
            try
            {
                // ptj1

            }
            catch (IndexBatchException ex)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.Error.WriteLine("Index batch exception: " + ex);
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error processing tweet: " + tweet + ". Exception: " + ex);
                throw;
            }
        }
    }
}
