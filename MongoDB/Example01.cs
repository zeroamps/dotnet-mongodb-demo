using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example01;

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("devices");

        var documents = new[] {
                new BsonDocument {
                    { "name", "Temperature" },
                    { "measurements", new BsonArray { -10, -5, -3, 0, 1, 3, 7, 10, 13, 17, 23, 28, 36 } }
                },
                new BsonDocument  {
                    { "name", "Humidity" },
                    { "measurements", new BsonArray { 10, 40, 30, 80, 60, 35, 70, 90, 15, 25, 60 } }
                }
            };

        //await collection.InsertManyAsync(documents);

        Console.WriteLine(await collection.CountDocumentsAsync(new BsonDocument { }));
        Console.WriteLine(await collection.CountDocumentsAsync(Builders<BsonDocument>.Filter.Empty));
        Console.WriteLine();

        Console.WriteLine((await collection.Find(new BsonDocument { }).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(Builders<BsonDocument>.Filter.Empty).ToListAsync()).ToJson());
        Console.WriteLine();

        var filter = Builders<BsonDocument>.Filter.Eq("name", "Temperature");
        Console.WriteLine((await collection.Find(filter).ToListAsync()).ToJson());
        Console.WriteLine();

        var builder = Builders<BsonDocument>.Filter;
        filter = builder.Eq("name", "Temperature") & builder.AnyEq("measurements", 10);
        Console.WriteLine((await collection.Find(filter).ToListAsync()).ToJson());
        Console.WriteLine();

        var sort = Builders<BsonDocument>.Sort.Descending("age");
        Console.WriteLine((await collection.Find(new BsonDocument { }).Sort(sort).ToListAsync()).ToJson());
        Console.WriteLine();

        var projection = Builders<BsonDocument>.Projection.Exclude("_id");
        Console.WriteLine((await collection.Find(new BsonDocument { }).Project(projection).ToListAsync()).ToJson());
        Console.WriteLine();
    }
}
