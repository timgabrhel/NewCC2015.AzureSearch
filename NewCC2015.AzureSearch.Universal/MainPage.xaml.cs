using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NewCC2015.AzureSearch.Universal.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NewCC2015.AzureSearch.Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _searched;
        private bool _cleared;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SearchBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Search();
        }

        private async void btnApplyFilters_Tapped(object sender, TappedRoutedEventArgs args)
        {
            await Search();
        }

        private void btnClearFilters_Tapped(object sender, TappedRoutedEventArgs args)
        {
            Clear();
        }

        private async Task Search()
        {
            var viewModel = DataContext as MainViewModel;

            // button click is firing twice for some reason, so keep track
            if (_searched == false)
            {
                _searched = true;

                // work around because the search box doesn't have command binding
                viewModel.SearchCommand.Execute(null);
            }
            else
            {
                // reset back to false for future clicks
                _searched = false;
            }
        }

        private void Clear()
        {
            var viewModel = DataContext as MainViewModel;

            // button click is firing twice for some reason, so keep track
            if (_cleared == false)
            {
                // work around because the search box doesn't have command binding    
                viewModel.ClearFiltersCommand.Execute(null);

                _cleared = true;
            }
            else
            {
                // reset back to false for future clicks
                _cleared = false;
            }
        }

        private async void SearchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            //http://azure.microsoft.com/blog/2015/01/20/azure-search-how-to-add-suggestions-auto-complete-to-your-search-applications/
            // require at least 3 characters
            // http://stackoverflow.com/questions/20092587/win-8-1-searchbox-binding-suggestions

            var viewModel = DataContext as MainViewModel;
            if (viewModel == null) return;

            if (viewModel.SearchString == null || viewModel.SearchString.Length < 3) return;

            var deferral = args.Request.GetDeferral();

            try
            {
                var suggestions = await viewModel.SearchSuggest();

                args.Request.SearchSuggestionCollection.AppendQuerySuggestions(suggestions.Select(s => s.Text));
                args.Request.SearchSuggestionCollection.AppendSearchSeparator("Top Tweeters");

                var imageUri = new Uri("ms-appx:///Assets/Azure.png");
                var imageRef = RandomAccessStreamReference.CreateFromUri(imageUri);

                foreach (var suggestResult in suggestions)
                {
                    args.Request.SearchSuggestionCollection.AppendResultSuggestion(suggestResult.Document.ScreenName, suggestResult.Document.Followers + " followers", "", imageRef, "");
                }
            }
            finally
            {
                deferral.Complete();
            }

        }
    }
}
