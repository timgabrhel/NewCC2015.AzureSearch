using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using GabrhelDigital.Universal.Core.Binding;
using Microsoft.Azure.Search.Models;
using NewCC2015.AzureSearch.Core;
using NewCC2015.AzureSearch.Universal.Model;
using Newtonsoft.Json.Serialization;

namespace NewCC2015.AzureSearch.Universal.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<MarchMadnessTweet> _tweets;
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

        public ObservableCollection<MarchMadnessTweet> Tweets
        {
            get { return _tweets; }
            set { SetProperty(ref _tweets, value); }
        }

        public string SearchString
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        public ObservableCollection<string> SearchableFields { get; } = new ObservableCollection<string>()
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

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            SearchField = "text";
        }

        private void Search()
        {
            var client = new AzureSearchClient();

            DocumentSearchResponse<MarchMadnessTweet> result = null;
            
            Task.Run(async () =>
            {
                result = await client.Search<MarchMadnessTweet>(SearchString, null, SearchField);
            }).Wait();
            
            Tweets = result.Results.Select(r => r.Document).ToObservableCollection();
            SourceFacets = result.Facets["source"].ToObservableCollection();
            RetweetsFacets = result.Facets["retweets"].ToObservableCollection();
            FollowingFacets = result.Facets["following"].ToObservableCollection();
            FollowersFacets = result.Facets["followers"].ToObservableCollection();

            OnPropertyChanged("SearchResult");
        }
    }
}
