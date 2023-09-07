using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example09;

internal class Bus
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Bus>("buses");

        var buses = new List<Bus>();
        for (int i = 0; i < 10000; i++)
        {
            buses.Add(new Bus() { Name = $"Bus {i}" });
        }
        await collection.InsertManyAsync(buses);

        using (var result = await collection.FindAsync(_ => true, new FindOptions<Bus> { BatchSize = 225 }))
        {
            while (await result.MoveNextAsync())
            {
                Console.WriteLine(result.Current.Select(f => f.Name).ToJson());
                Console.WriteLine();
            }
        }

        await collection.DeleteManyAsync(_ => true);
    }
}
