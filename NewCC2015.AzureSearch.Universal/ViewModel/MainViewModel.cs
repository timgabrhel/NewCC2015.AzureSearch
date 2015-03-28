using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GabrhelDigital.Universal.Core.Binding;
using Microsoft.Azure.Search.Models;
using NewCC2015.AzureSearch.Core;
using NewCC2015.AzureSearch.Universal.Model;
using Newtonsoft.Json.Serialization;

namespace NewCC2015.AzureSearch.Universal.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<SearchResult<MarchMadnessTweet>> _tweets;
        private string _searchString;
        private string _searchField;
        private ObservableCollection<Facet> _sourceFacets;
        private ObservableCollection<Facet> _retweetsFacets;
        private ObservableCollection<Facet> _followersFacets;
        private ObservableCollection<Facet> _followingFacets;
        private Facet _sourceFacet;
        private Facet _retweetsFacet;
        private Facet _followersFacet;
        private Facet _followingFacet;
        private bool _isBusy;
        private string _searchExecutionTime;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public string SearchExecutionTime
        {
            get { return _searchExecutionTime; }
            set { SetProperty(ref _searchExecutionTime, value); }
        }

        public ObservableCollection<SearchResult<MarchMadnessTweet>> Tweets
        {
            get { return _tweets; }
            set { SetProperty(ref _tweets, value); OnPropertyChanged("SearchResult"); }
        }

        public string SearchString
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        public ObservableCollection<string> SearchableFields
        { get; }
        = new ObservableCollection<string>()
        {
            "source",
            "text",
            "screenName"
        };

        public string SearchField
        {
            get { return _searchField; }
            set { SetProperty(ref _searchField, value); }
        }

        public string SearchResult => Tweets == null ? "" : string.Format("{0} tweets found", Tweets.Count);

        public ObservableCollection<Facet> SourceFacets
        {
            get { return _sourceFacets; }
            set { SetProperty(ref _sourceFacets, value); }
        }

        public ObservableCollection<Facet> RetweetsFacets
        {
            get { return _retweetsFacets; }
            set { SetProperty(ref _retweetsFacets, value); }
        }

        public ObservableCollection<Facet> FollowersFacets
        {
            get { return _followersFacets; }
            set { SetProperty(ref _followersFacets, value); }
        }

        public ObservableCollection<Facet> FollowingFacets
        {
            get { return _followingFacets; }
            set { SetProperty(ref _followingFacets, value); }
        }

        public Facet SourceFacet
        {
            get { return _sourceFacet; }
            set { SetProperty(ref _sourceFacet, value); }
        }

        public Facet RetweetsFacet
        {
            get { return _retweetsFacet; }
            set { SetProperty(ref _retweetsFacet, value); }
        }

        public Facet FollowersFacet
        {
            get { return _followersFacet; }
            set { SetProperty(ref _followersFacet, value); }
        }

        public Facet FollowingFacet
        {
            get { return _followingFacet; }
            set { SetProperty(ref _followingFacet, value); }
        }

        public ICommand SearchCommand { get; private set; }

        public ICommand ClearFiltersCommand { get; private set; }


        public MainViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            SearchField = "text";
        }

        private void Search()
        {
            IsBusy = true;

            // a search string is required. if it's empty, set it to "all"
            if (string.IsNullOrWhiteSpace(SearchString))
            {
                SearchString = "*";
            }

            // create our search client
            var client = new AzureSearchClient();

            // after we load the search results & set the facet collections, two way binding will clear the currently selected facet.
            // we'll store these temporarily and reapply them later
            var sourceFacet = SourceFacet;
            var retweetsFacet = RetweetsFacet;
            var followingFacet = FollowingFacet;
            var followersFacet = FollowersFacet;


            DocumentSearchResponse<MarchMadnessTweet> searchResult = null;
            var executionTime = string.Empty;

            Task.Run(async delegate
            {
                var start = DateTime.Now;
                searchResult = await client.Search<MarchMadnessTweet>(SearchString, SearchField, SourceFacet, RetweetsFacet, FollowingFacet, FollowersFacet);
                var end = DateTime.Now;

                executionTime = (end.Subtract(start)).TotalMilliseconds.ToString("#####.##") + " milliseconds";
            }).Wait();

            SearchExecutionTime = executionTime;
            Tweets = searchResult.Results.ToObservableCollection();
            SourceFacets = searchResult.Facets["source"].Where(f => f.Count > 0).ToObservableCollection();
            RetweetsFacets = searchResult.Facets["retweets"].Where(f => f.Count > 0).ToObservableCollection();
            FollowingFacets = searchResult.Facets["following"].Where(f => f.Count > 0).ToObservableCollection();
            FollowersFacets = searchResult.Facets["followers"].Where(f => f.Count > 0).ToObservableCollection();

            SourceFacet = sourceFacet;
            RetweetsFacet = retweetsFacet;
            FollowingFacet = followingFacet;
            FollowersFacet = followersFacet;

            IsBusy = false;
        }

        private void ClearFilters()
        {
            IsBusy = true;

            SourceFacet = null;
            RetweetsFacet = null;
            FollowingFacet = null;
            FollowersFacet = null;

            Search();

            IsBusy = false;
        }

        public async Task<DocumentSuggestResponse<MarchMadnessTweet>> SearchSuggest()
        {
            IsBusy = true;

            if (SearchString.Length < 3)
            {
                return null;
            }

            // create our search client
            var client = new AzureSearchClient();

            // get the results
            var result = await client.Suggest<MarchMadnessTweet>(SearchString);

            IsBusy = false;

            return result;
        }

    }
}
