using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NewCC2015.AzureSearch.Universal
{
    public static class Utility
    {
        public static T GetResource<T>(String resourceName) where T : class
        {
            T value = default(T);

            if (App.Current.Resources.ContainsKey(resourceName))
            {
                try
                {
                    value = App.Current.Resources[resourceName] as T;
                }
                catch
                {
                }
            }
            return value;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }
    }
}
