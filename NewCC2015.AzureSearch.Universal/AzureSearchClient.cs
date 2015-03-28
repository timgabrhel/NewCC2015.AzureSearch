using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using NewCC2015.AzureSearch.Core;
using NewCC2015.AzureSearch.Universal.Model;

namespace NewCC2015.AzureSearch.Universal
{
    public class AzureSearchClient
    {
        private string searchServiceName;
        private string searchIndex;
        private string searchApiKey;
        private SearchCredentials searchCredentials;
        private SearchServiceClient searchClient;

        public AzureSearchClient()
        {
            searchServiceName = Utility.GetResource<string>("SearchServiceName");
            searchIndex = Utility.GetResource<string>("SearchIndexName");
            searchApiKey = Utility.GetResource<string>("SearchApiKey");

            searchCredentials = new SearchCredentials(searchApiKey);
            searchClient = new SearchServiceClient(searchServiceName, searchCredentials);
        }

        public async Task<DocumentSearchResponse<T>> Search<T>(string searchText, string field, Facet sourceFacet, Facet retweetsFacet, Facet followingFacet, Facet followersFacet) where T : class
        {
            // client to work against the index
            var indexClient = searchClient.Indexes.GetClient(searchIndex);

            // build the filter string based on any selected facets
            var filter = new StringBuilder();

            if (sourceFacet != null)
            {
                filter.Append("source eq '" + sourceFacet.Value + "'");
            }

            if (retweetsFacet != null)
            {
                if (retweetsFacet.From != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("retweets ge " + retweetsFacet.From);
                }
                if (retweetsFacet.To != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("retweets le " + retweetsFacet.To);
                }
            }

            if (followingFacet != null)
            {
                if (followingFacet.From != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("following ge " + followingFacet.From);
                }
                if (followingFacet.To != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("following le " + followingFacet.To);
                }
            }

            if (followersFacet != null)
            {
                if (followersFacet.From != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("followers ge " + followersFacet.From);
                }
                if (followersFacet.To != null)
                {
                    ConditionalAppendAnd(filter);
                    filter.Append("followers le " + followersFacet.To);
                }
            }

            // create the search object including the field(s) to search, filter, etc.
            var sp = new SearchParameters
            {
                //Select = The fields to return. Defaults to all defined in schema
                Top = 1000, // max we can get in 1 query, use paging approach to continue gathering items

                // Field(s) we want to search against. 
                SearchFields = new[] { field },

                // Highlight the text in the result that was matched by the search string
                HighlightFields = new[] { field },

                // pre & post fix the highlighted text with these elements
                HighlightPreTag = "",
                HighlightPostTag = "",

                // by passing delimited values, search will return item counts within reach range for each facet.
                // because source is a string field, each unique value with count will be displayed.
                Facets = new List<string>()
                {
                    "source",
                    "retweets,values:5|15|50|100",
                    "following,values:25|50|100|200|500|1000|5000|10000",
                    "followers,values:50|100|250|500|1000|2500|5000|10000|25000|50000|100000|500000"
                }
            };

            // apply the filter if one exists. this cannot be empty.
            if (filter.Length > 0)
            {
                sp.Filter = filter.ToString();
            }
            
            // execute the search request
            return await indexClient.Documents.SearchAsync<T>(searchText, sp);
        }

        public async Task<DocumentSuggestResponse<T>> Suggest<T>(string searchText) where T : class 
        {
            var indexClient = searchClient.Indexes.GetClient(searchIndex);

            // pass the search text, the name of the suggester defined in the index
            return await indexClient.Documents.SuggestAsync<T>(searchText, "textSuggester",
                new SuggestParameters()
                {
                    UseFuzzyMatching = true,
                    OrderBy = new[] {"followers desc"}
                });
        }

        private void ConditionalAppendAnd(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Append(" and ");
            }
        }
    }
}
