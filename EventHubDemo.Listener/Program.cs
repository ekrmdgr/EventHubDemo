using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using System.Threading;

namespace EventHubDemo.Listener
{
    class Program
    {
        private const string ehubNamespaceConnectionString = "Endpoint=sb://testeventhub2021.servicebus.windows.net/;SharedAccessKeyName=eventhub;SharedAccessKey=dpDp+H+RKbIkqsLhzYmqlCSzJY9SI/WNm/e3vFk+1Ys=;EntityPath=eventhubdemo";
        private const string eventHubName = "eventhubdemo";
        private const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=testblobstorage2021;AccountKey=hN77k/JHnRt+lGysIHJHuP2U9dvbIrYkPuwWrBN0O7ouj6mbQ47syeBxzmjvnPNugzVCXVancO8mVMXL8E0G8Q==;EndpointSuffix=core.windows.net";
        private const string blobContainerName = "blobcontainer";
        static async Task Main()
        {
            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            await Task.Delay(Timeout.InfiniteTimeSpan);

            // Stop the processing
            await processor.StopProcessingAsync();
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tRecevied event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
