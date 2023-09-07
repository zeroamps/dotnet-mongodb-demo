using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example06;

internal class Car
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string[] Users { get; set; }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Car>("cars");

        var documents = new[] {
            new Car {
                Name = "User A",
                Users = new [] { "A", "B", "C" }
            },
            new Car  {
                Name = "User B",
                Users = new [] { "A", "B", "C" }
            }
        };

        await collection.InsertManyAsync(documents);

        //var updateDefinition = Builders<Car>.Update.Set(f => f.Users, new[] { "X", "Y", "Z" });
        var updateDefinition = Builders<Car>.Update.AddToSetEach(f => f.Users, new[] { "B", "D", "E" });
        //var updateDefinition = Builders<Car>.Update.Push(f => f.Users, "E");
        //var updateDefinition = Builders<Car>.Update.Push(f => f.Users, "A");
        //var updateDefinition = Builders<Car>.Update.PushEach(f => f.Users, new[] { "A", "B", "K", "L" });

        await collection.UpdateManyAsync(Builders<Car>.Filter.Empty, updateDefinition);
        Console.WriteLine((await collection.Find(Builders<Car>.Filter.Empty).ToListAsync()).ToJson());

        await collection.DeleteManyAsync(Builders<Car>.Filter.Empty);
    }
}
