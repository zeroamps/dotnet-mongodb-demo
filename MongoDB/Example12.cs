using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example12;

internal class Activity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<ActivitySession> Sessions { get; set; }

    public Activity()
    {
        Sessions = new List<ActivitySession>();
    }
}

internal class ActivitySession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<SessionExercise> Exercises { get; set; }

    public ActivitySession()
    {
        Exercises = new List<SessionExercise>();
    }
}

internal class SessionExercise
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<Challenge> Challenges { get; set; }
    public SessionExercise()
    {
        Challenges = new List<Challenge>();
    }
}

internal class Challenge
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
        var collection = database.GetCollection<Activity>("activities");

        await collection.InsertManyAsync(new[]
        {
            new Activity {
                Name = "Activity I",
                Sessions = new []
                {
                    new ActivitySession
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = "Session I",
                        Exercises = new []
                        {
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise I",
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise II",
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise III",
                            }
                        }
                    },
                    new ActivitySession
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = "Session II",
                        Exercises = new []
                        {
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise I",
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise II",
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise III",
                            }
                        }
                    }
                }
            }
        }); ;

        var filterDefinition = Builders<Activity>.Filter.Eq(f => f.Name, "Activity I")
            & Builders<Activity>.Filter.ElemMatch(f => f.Sessions, f => f.Name == "Session I");

        var updates = new List<UpdateDefinition<Activity>>();
        for (int i = 0; i < 3; i++)
        {
            updates.Add(Builders<Activity>.Update.AddToSet($"Sessions.$.Exercises.{i}.Challenges",
               new Challenge { Id = ObjectId.GenerateNewId().ToString(), Name = "TEST" }));
        }

        var updateDefinition = Builders<Activity>.Update.Combine(updates);

        var result = await collection.FindOneAndUpdateAsync(filterDefinition, updateDefinition,
            new FindOneAndUpdateOptions<Activity> { ReturnDocument = ReturnDocument.After });
        Console.WriteLine(result.ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
