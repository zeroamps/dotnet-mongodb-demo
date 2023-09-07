using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example05;

internal class Teacher
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
        var collection = database.GetCollection<Teacher>("teacher");

        await collection.InsertManyAsync(new List<Teacher> {
            new Teacher { FirstName = "Pepo 1", LastName = "Lovak 1", Age = 20 },
            new Teacher { FirstName = "Pepo 2", LastName = "Lovak 1", Age = 30 },
            new Teacher { FirstName = "Pepo 3", LastName = "Lovak 1", Age = 40 },
            new Teacher { FirstName = "Pepo 1", LastName = "Lovak 2", Age = 20 },
            new Teacher { FirstName = "Pepo 2", LastName = "Lovak 2", Age = 30 },
            new Teacher { FirstName = "Pepo 3", LastName = "Lovak 2", Age = 40 },
            new Teacher { FirstName = "Pepo 1", LastName = "Lovak 3", Age = 20 },
            new Teacher { FirstName = "Pepo 2", LastName = "Lovak 3", Age = 30 },
            new Teacher { FirstName = "Pepo 3", LastName = "Lovak 3", Age = 40 }
        });

        // Approach 1
        var sortDefinition = Builders<Teacher>.Sort.Descending(f => f.Age).Descending(f => f.LastName);
        Console.WriteLine((await collection.Find(_ => true).Sort(sortDefinition).ToListAsync()).ToJson());

        // Approach 2
        Console.WriteLine((await collection.Find(_ => true)
            .SortByDescending(f => f.Age).ThenByDescending(f => f.LastName).ToListAsync()).ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
