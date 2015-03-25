using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewCC2015.AzureSearch.Core;
using Newtonsoft.Json;

namespace NewCC2015.AzureSearch.GetTweetsJob
{
    public class Program
    {
        static readonly ApplicationOnlyAuthorizer Auth = new ApplicationOnlyAuthorizer
        {
            CredentialStore = new InMemoryCredentialStore()
            {
                ConsumerKey = ConfigurationManager.AppSettings["TwitterKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["TwitterSecret"]
            }
        };

        private static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static CloudQueue _queue = null;
        
        private static ulong _lastStatusId = 0;

        public static void Main()
        {
            Task.Run(async () =>
            {
                // initialize linq to twitter
                await Auth.AuthorizeAsync();

                // initialize the queue
                InitializeTweetQueue();

                // get tweets endlessly
                while (true)
                {
                    GetTweets();

                    await Task.Delay(TimeSpan.FromMinutes(10));
                }
            }).Wait();
        }

        private static void GetTweets()
        {
            try
            {
                // connect to twitter
                var context = new TwitterContext(Auth);

                // create a search instance by looking for #MarchMadness
                var search =
                    (from s in context.Search
                     where s.Type == SearchType.Search &&
                           s.Query == "#MarchMadness" &&
                           s.SinceID == _lastStatusId &&
                           s.Count == 100
                     select s).SingleOrDefault();

                // ensure the search instance isn't null
                if (search == null)
                {
                    Console.Out.WriteLine("Twitter Search instance is null");
                    return;
                }

                if (search.Statuses.Count <= 0)
                {
                    return;
                }

                // loop over every status returned
                Parallel.ForEach(search.Statuses, async delegate(Status status)
                {
                    try
                    {
                        await SaveStatus(status);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.ToString());
                    }
                });

                _lastStatusId = search.Statuses.Last().StatusID;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        private static async Task SaveStatus(Status status)
        {
            // create a poco from the status
            var tweet = new MarchMadnessTweet()
            {
                TweetId = status.StatusID.ToString(),
                Source = GetSourceNameFromAnchor(status.Source),
                Text = status.Text,
                CreatedAt = status.CreatedAt,
                ScreenName = status.User.ScreenNameResponse,
                ProfileBackgroundColor = status.User.ProfileBackgroundColor,
                ProfileImageUrl = status.User.ProfileImageUrl,
                TimeZone = status.User.TimeZone,
                Followers = status.User.FollowersCount,
                Following = status.User.FriendsCount,
                Retweets = status.RetweetCount,
                Statuses = status.User.StatusesCount
            };

            // convert to queue message and save
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(tweet));
            await _queue.AddMessageAsync(message);
        }

        private static string GetSourceNameFromAnchor(string source)
        {
            return source.Substring(source.IndexOf(">", StringComparison.Ordinal) + 1, source.IndexOf("</a>", StringComparison.Ordinal) - source.IndexOf(">", StringComparison.Ordinal) - 1);
        }

        private static void InitializeTweetQueue()
        {
            var queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference(Strings.MarchMadnessQueueName);
            _queue.CreateIfNotExists();
        }
    }
}
