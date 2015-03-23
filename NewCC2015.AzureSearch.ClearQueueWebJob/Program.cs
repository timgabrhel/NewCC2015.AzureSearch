using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewCC2015.AzureSearch.Core;

namespace NewCC2015.AzureSearch.ClearQueueWebJob
{
    public class Program
    {
        private static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static CloudQueue _queue = null;

        public static void Main()
        {
            var queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference(Strings.MarchMadnessQueueName);
            _queue.Clear();
        }
    }
}
