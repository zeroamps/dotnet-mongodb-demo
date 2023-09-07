using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.Example08;

internal class Train
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}

internal class Main
{      
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Train>("trains");

        await collection.InsertManyAsync(new[] {
            new Train() { Code = "101", Name = "Train A" },
            new Train() { Code = "102", Name = "Train B" },
            new Train() { Code = "103", Name = "Train C" },
            new Train() { Code = "104", Name = "Train D" },
            new Train() { Code = "105", Name = "Train E" }
        });

        Console.WriteLine((await collection.Find(_ => true).ToListAsync()).ToJson());

        var indexKeysDefinition = new CreateIndexModel<Train>(
            Builders<Train>.IndexKeys.Ascending(f => f.Code), new CreateIndexOptions { Unique = true });
        var indexCodeName = await collection.Indexes.CreateOneAsync(indexKeysDefinition);
        Console.WriteLine((await collection.Indexes.ListAsync()).ToJson());
        await collection.Indexes.DropOneAsync(indexCodeName);

        await collection.DeleteManyAsync(_ => true);
    }
}
