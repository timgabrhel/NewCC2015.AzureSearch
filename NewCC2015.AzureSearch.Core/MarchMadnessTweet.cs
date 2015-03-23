using System;
using Microsoft.Azure.Search.Models;

namespace NewCC2015.AzureSearch.Core
{
    [SerializePropertyNamesAsCamelCase]
    public class MarchMadnessTweet
    {
        public string TweetId { get; set; }

        public string Source { get; set; }

        public string Text { get; set; }

        public string ScreenName { get; set; }

        public string ProfileBackgroundColor { get; set; }

        public string ProfileImageUrl { get; set; }

        public string TimeZone { get; set; }

        public long Followers { get; set; }

        public long Following { get; set; }

        public long Retweets { get; set; }

        public long Statuses { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public override string ToString()
        {
            return string.Format("@{0}: {1}", ScreenName, Text);
        }
    }
}
