using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubDemo.TestApplication
{
    class Program
    {
        private const string connectionString = "Endpoint=sb://testeventhub2021.servicebus.windows.net/;SharedAccessKeyName=eventhub;SharedAccessKey=dpDp+H+RKbIkqsLhzYmqlCSzJY9SI/WNm/e3vFk+1Ys=;EntityPath=eventhubdemo";
        private const string eventHubName = "eventhubdemo";
        static async Task Main()
        {
            // Create a producer client that you can use to send events to an event hub
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                // eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("First event")));
                // eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Second event")));
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Third event")));

                // Use the producer client to send the batch of events to the event hub
                while (true)
                {
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("A batch of 1 events has been published.");
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
