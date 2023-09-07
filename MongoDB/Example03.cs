using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.Example03;

internal class Author
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        // Approach 1
        var collection = database.GetCollection<Author>("authors");

        await collection.InsertOneAsync(new Author { FirstName = "Pepo 1", LastName = "Lovak 1" });
        await collection.InsertOneAsync(new Author { FirstName = "Pepo 2", LastName = "Lovak 2" });
        await collection.InsertOneAsync(new Author { FirstName = "Pepo 3", LastName = "Lovak 3" });

        var filterDefinition = Builders<Author>.Filter.Empty;
        var authors = await collection.Find(filterDefinition).ToListAsync();
        Console.WriteLine(authors.ToJson());

        filterDefinition = Builders<Author>.Filter.Eq(f => f.Id, authors[0].Id);
        var updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 101");
        await collection.UpdateOneAsync(filterDefinition, updateDefinition);
        Console.WriteLine((await collection.Find(filterDefinition).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(filterDefinition);

        filterDefinition = Builders<Author>.Filter.Eq(f => f.Id, authors[1].Id);
        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 102");
        await collection.UpdateOneAsync(filterDefinition, updateDefinition);
        Console.WriteLine((await collection.Find(filterDefinition).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(filterDefinition);

        filterDefinition = Builders<Author>.Filter.Eq(f => f.Id, authors[2].Id);
        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 103");
        await collection.UpdateOneAsync(filterDefinition, updateDefinition);
        Console.WriteLine((await collection.Find(filterDefinition).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(filterDefinition);


        await collection.InsertManyAsync(new List<Author> {
            new Author { FirstName = "Pepo 1", LastName = "Lovak 1" },
            new Author { FirstName = "Pepo 2", LastName = "Lovak 2" },
            new Author { FirstName = "Pepo 3", LastName = "Lovak 3" }
        });

        filterDefinition = Builders<Author>.Filter.Regex(f => f.FirstName, "Pepo");
        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepoooooo");
        await collection.UpdateManyAsync(filterDefinition, updateDefinition);
        filterDefinition = Builders<Author>.Filter.Empty;
        Console.WriteLine((await collection.Find(filterDefinition).ToListAsync()).ToJson());

        await collection.DeleteManyAsync(filterDefinition);


        // Approach 2
        collection = database.GetCollection<Author>("authors");

        await collection.InsertOneAsync(new Author { FirstName = "Pepo 1", LastName = "Lovak 1" });
        await collection.InsertOneAsync(new Author { FirstName = "Pepo 2", LastName = "Lovak 2" });
        await collection.InsertOneAsync(new Author { FirstName = "Pepo 3", LastName = "Lovak 3" });

        filterDefinition = Builders<Author>.Filter.Empty;
        authors = await collection.Find(_ => true).ToListAsync();
        Console.WriteLine(authors.ToJson());

        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 101");
        await collection.UpdateOneAsync(f => f.Id == authors[0].Id, updateDefinition);
        Console.WriteLine((await collection.Find(f => f.Id == authors[0].Id).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(f => f.Id == authors[0].Id);

        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 102");
        await collection.UpdateOneAsync(f => f.Id == authors[1].Id, updateDefinition);
        Console.WriteLine((await collection.Find(f => f.Id == authors[1].Id).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(f => f.Id == authors[1].Id);

        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepo 103");
        await collection.UpdateOneAsync(f => f.Id == authors[2].Id, updateDefinition);
        Console.WriteLine((await collection.Find(f => f.Id == authors[2].Id).ToListAsync()).ToJson());
        await collection.DeleteOneAsync(f => f.Id == authors[2].Id);


        await collection.InsertManyAsync(new List<Author> {
            new Author { FirstName = "Pepo 1", LastName = "Lovak 1" },
            new Author { FirstName = "Pepo 2", LastName = "Lovak 2" },
            new Author { FirstName = "Pepo 3", LastName = "Lovak 3" }
        });

        updateDefinition = Builders<Author>.Update.Set(u => u.FirstName, "Pepoooooo");
        await collection.UpdateManyAsync(f => f.FirstName.StartsWith("Pepo"), updateDefinition);
        Console.WriteLine((await collection.Find(_ => true).ToListAsync()).ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
