using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace EventstoreSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("StTarting es connection");

            var connectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";
            var client = EventStoreConnection.Create(connectionString);
            client.Connected += (object sender, ClientConnectionEventArgs arg) =>
            {
                var catchUpSubscriptionSettings =
                    new CatchUpSubscriptionSettings(4096, 4096, false, true, "shopCategorySubscription");
                client.SubscribeToStreamFrom("$ce-shop", 0, catchUpSubscriptionSettings,
                    OnEvent, OnProcessingstarted, OnDropped);
            };

            client.ConnectAsync();

            Console.WriteLine("waiting for events!");
            Console.ReadLine();
        }


        private static void OnProcessingstarted(EventStoreCatchUpSubscription sub)
        {
            Console.WriteLine("Caught up, live processing started");
        }

        private static void OnDropped(EventStoreCatchUpSubscription sub, SubscriptionDropReason reason, Exception err)
        {
            Console.WriteLine($"Connection Dropped: ${reason}, error: ${err.Message}");
        }

        static Task OnEvent(EventStoreCatchUpSubscription sub, ResolvedEvent ev)
        {
            return Task.Run(() =>
                Console.WriteLine($"EventType: {ev.Event.EventType}, EventNumber: {ev.OriginalEventNumber}"));
//            Console.WriteLine("saw event");
//            return new Task(() =>
//                );
        }
    }
}