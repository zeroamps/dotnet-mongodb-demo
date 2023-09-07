using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example11;

internal class Button
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<string> Clicks { get; set; }

    public IEnumerable<string> DoubleClicks { get; set; }

    public IEnumerable<Session> Sessions { get; set; }

    public Button()
    {
        Clicks = new List<string>();
        DoubleClicks = new List<string>();
        Sessions = new List<Session>();
    }
}

internal class Session
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<string> Clicks { get; set; }

    public IEnumerable<string> DoubleClicks { get; set; }

    public Session()
    {
        Clicks = new List<string>();
        DoubleClicks = new List<string>();
    }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Button>("buttons");

        await collection.InsertManyAsync(new[]
        {
            new Button {
                Name = "Button 1",
                Clicks = new []
                {
                   "Click 1",
                   "Click 2",
                   "Click 3",
                   "Click 4",
                   "Click 5"
                },
                DoubleClicks = new []
                {
                   "Double Click 1",
                   "Double Click 2",
                   "Double Click 3",
                   "Double Click 4",
                   "Double Click 5"
                },
                Sessions = new[]
                {
                    new Session {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = "Session 1",
                        Clicks = new []
                        {
                            "Click 1",
                            "Click 2",
                            "Click 3",
                            "Click 4",
                            "Click 5"
                        },
                        DoubleClicks = new []
                        {
                            "Double Click 1",
                            "Double Click 2",
                            "Double Click 3",
                            "Double Click 4",
                            "Double Click 5"
                        }
                    },
                    new Session {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = "Session 2",
                        Clicks = new []
                        {
                            "Click 1",
                            "Click 2",
                            "Click 3",
                            "Click 4",
                            "Click 5"
                        },
                        DoubleClicks = new []
                        {
                            "Double Click 1",
                            "Double Click 2",
                            "Double Click 3",
                            "Double Click 4",
                            "Double Click 5"
                        }
                    }
                }
            }
        });

        var filterDefinition = Builders<Button>.Filter.Eq(f => f.Name, "Button 1")
            & Builders<Button>.Filter.ElemMatch(f => f.Sessions, f => f.Name == "Session 1");

        var updateDefinition = Builders<Button>.Update
            .Pull(f => f.Clicks, "Click 5")
            .AddToSet(f => f.DoubleClicks, "Double Click 6")
            .Pull("Sessions.$.Clicks", "Click 5")
            .AddToSet("Sessions.$.DoubleClicks", "Double Click 6");

        var button = await collection.FindOneAndUpdateAsync(filterDefinition, updateDefinition,
            new FindOneAndUpdateOptions<Button> { ReturnDocument = ReturnDocument.After });
        Console.WriteLine(button.ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
