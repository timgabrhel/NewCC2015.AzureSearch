using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using GabrhelDigital.Universal.Core.Binding;
using NewCC2015.AzureSearch.Core;
using NewCC2015.AzureSearch.Universal.Model;

namespace NewCC2015.AzureSearch.Universal.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<MarchMadnessTweet> _tweets;
        private string _searchString;
        private SearchFilter _filter;

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

        public ObservableCollection<SearchFilter> Filters { get; } = new ObservableCollection<SearchFilter>()
        {
            new SearchFilter("Comment", "Tweet", "text"),
            new SearchFilter("Globe", "Source", "source"),
            new SearchFilter("People", "Username", "screenname")
        };

        public SearchFilter Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }

        public string SearchResult => Tweets == null ? "" : string.Format("{0} tweets found", Tweets.Count);

        public ICommand SearchCommand { get; private set; }

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(Search);
        }

        private void Search()
        {
            var client = new AzureSearchClient();

            List<MarchMadnessTweet> tweets = null;

            Task.Run(async () =>
            {
                var result = await client.Search<MarchMadnessTweet>(SearchString);
                tweets = result.Results.Select(r => r.Document).ToList();
            }).Wait();

            Tweets = new ObservableCollection<MarchMadnessTweet>(tweets);
            OnPropertyChanged("SearchResult");
        }
    }
}
