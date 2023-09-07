using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.Example04;

internal class Student
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Student>("students");

        await collection.InsertManyAsync(new List<Student> {
            new Student { FirstName = "Pepo 1", LastName = "Lovak 1", Age = 20 },
            new Student { FirstName = "Pepo 2", LastName = "Lovak 2", Age = 30 },
            new Student { FirstName = "Pepo 3", LastName = "Lovak 3", Age = 40 }
        });

        var filterDefinition = Builders<Student>.Filter.Eq(f => f.FirstName, "Pepo 3");
        var updateDefinition = Builders<Student>.Update
            .Set(u => u.FirstName, "Pepo 333")
            .Set(u => u.LastName, "Lovak 333")
            .Set(u => u.Age, 333);
        await collection.UpdateOneAsync(filterDefinition, updateDefinition, new UpdateOptions { IsUpsert = true });

        filterDefinition = Builders<Student>.Filter.Eq(f => f.FirstName, "Pepo 4");
        updateDefinition = Builders<Student>.Update
            .Set(u => u.FirstName, "Pepo 4")
            .Set(u => u.LastName, "Lovak 4")
            .Set(u => u.Age, 444);
        await collection.UpdateOneAsync(filterDefinition, updateDefinition, new UpdateOptions { IsUpsert = true });

        filterDefinition = Builders<Student>.Filter.Eq(f => f.FirstName, "Pepo 4");
        updateDefinition = Builders<Student>.Update
            .Set(u => u.FirstName, "Pepo 4")
            .Set(u => u.LastName, "Lovak 4")
            .Max(u => u.Age, 44);
        await collection.UpdateOneAsync(filterDefinition, updateDefinition, new UpdateOptions { IsUpsert = true });

        filterDefinition = Builders<Student>.Filter.Eq(f => f.FirstName, "Pepo 4");
        updateDefinition = Builders<Student>.Update
            .Set(u => u.FirstName, "Pepo 4")
            .Set(u => u.LastName, "Lovak 4")
            .Min(u => u.Age, 44);
        await collection.UpdateOneAsync(filterDefinition, updateDefinition, new UpdateOptions { IsUpsert = true });

        filterDefinition = Builders<Student>.Filter.Eq(f => f.FirstName, "Pepo 4");
        updateDefinition = Builders<Student>.Update
            .Set(u => u.FirstName, "Pepo 4")
            .Set(u => u.LastName, "Lovak 4")
            .Mul(u => u.Age, 4);
        Console.WriteLine((await collection.FindOneAndUpdateAsync(filterDefinition, updateDefinition,
            new FindOneAndUpdateOptions<Student> { ReturnDocument = ReturnDocument.After }))
            .ToJson());

        Console.WriteLine((await collection.Find(_ => true).ToListAsync()).ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
