using MongoDB.Driver;

namespace MongoDB
{
    internal class Program
    {
        async static Task Main()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("test");

            //await Example01.Main.Run(database);
            //await Example02.Main.Run(database);
            //await Example03.Main.Run(database);
            //await Example04.Main.Run(database);
            //await Example05.Main.Run(database);
            //await Example06.Main.Run(database);
            //await Example07.Main.Run(database);
            //await Example08.Main.Run(database);
            //await Example09.Main.Run(database);
            //await Example10.Main.Run(database);
            //await Example11.Main.Run(database);
            //await Example12.Main.Run(database);
            await Example13.Main.Run(database);
        }
    }
}