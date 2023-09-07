using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDB.Example07;

internal class User
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId[] Hobbies { get; set; }
    public Hobby[] HobbyObjects { get; set; }
}

internal class Hobby
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId Creator { get; set; }
    public User CreatorObject { get; set; }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var users = database.GetCollection<BsonDocument>("users");
        var hobbies = database.GetCollection<BsonDocument>("hobbies");

        var id = ObjectId.GenerateNewId();
        await hobbies.InsertManyAsync(new[] {
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby A" }, { "Creator", id } },
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby B" }, { "Creator", id } },
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby C" }, { "Creator", id } },
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby D" }, { "Creator", id } },
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby E" }, { "Creator", id } },
            new BsonDocument { { "Id", ObjectId.GenerateNewId() }, { "Name", "Hobby F" }, { "Creator", id } }
        });

        await users.InsertOneAsync(new BsonDocument { { "Id", id }, { "Name", "User A" } });

        var result = await users
            .Find(new BsonDocument { { "Name", "User A" } })
            .ToListAsync();
        Console.WriteLine(result.ToJson());
        Console.WriteLine();

        var pipeline = new BsonDocumentStagePipelineDefinition<BsonDocument, BsonDocument>(
             new[] {
                   new BsonDocument{ { "$match", new BsonDocument { { "Name", "User A" } } } }
             }
        );

        result = await users
            .Aggregate(pipeline)
            .ToListAsync();
        Console.WriteLine(result.ToJson());
        Console.WriteLine();

        pipeline = new BsonDocumentStagePipelineDefinition<BsonDocument, BsonDocument>(
            new[] {
                    new BsonDocument {
                        { "$lookup",
                            new BsonDocument {
                                { "from", "hobbies" },
                                { "localField", "Hobbies"},
                                { "foreignField", "_id" },
                                { "pipeline", new BsonArray {
                                    new BsonDocument {
                                        { "$lookup", new BsonDocument {
                                            { "from", "users" },
                                            { "localField", "Creator" },
                                            { "foreignField", "_id" },
                                            { "as", "CreatorObject" }
                                        }}
                                    }}
                                },
                                { "as", "HobbyObjects" }
                            }
                        }
                    },
                    new BsonDocument {
                        { "$unwind",
                            new BsonDocument {
                                { "path", "$HobbyObjects" }
                            }
                        }
                    },
                    new BsonDocument {
                        { "$unwind",
                            new BsonDocument {
                                { "path", "$HobbyObjects.CreatorObject" }
                            }
                        }
                    },
                    new BsonDocument {
                        { "$group",
                            new BsonDocument {
                                { "_id", "$_id" },
                                { "Name", new BsonDocument { { "$first", "$Name" } } },
                                { "Hobbies", new BsonDocument { { "$first", "$Hobbies" } } },
                                { "HobbyObjects", new BsonDocument { { "$push", "$HobbyObjects" } } }
                            }
                        }
                    }
            }
        );

        result = await users.Aggregate(pipeline).ToListAsync();
        Console.WriteLine(result.Select(document => BsonSerializer.Deserialize<User>(document)).ToList().ToJson());

        await hobbies.DeleteManyAsync(new BsonDocument { });
        await users.DeleteManyAsync(new BsonDocument { });
    }
}
