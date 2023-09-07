using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MongoDB.Example13;

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

        var objectIds = new[]
        {
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString()
        };

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
                                Challenges = new []
                                {
                                    new Challenge { Id = objectIds[0], Name = "Challenge I" },
                                    new Challenge { Id = objectIds[1], Name = "Challenge II" },
                                    new Challenge { Id = objectIds[2], Name = "Challenge III" }
                                }
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise II",
                                Challenges = new []
                                {
                                    new Challenge { Id = objectIds[3], Name = "Challenge I" },
                                    new Challenge { Id = objectIds[4], Name = "Challenge II" },
                                    new Challenge { Id = objectIds[5], Name = "Challenge III" }
                                }
                            },
                            new SessionExercise
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = "Exercise III",
                                Challenges = new []
                                {
                                    new Challenge { Id = objectIds[6], Name = "Challenge I" },
                                    new Challenge { Id = objectIds[7], Name = "Challenge II" },
                                    new Challenge { Id = objectIds[8], Name = "Challenge III" }
                                }
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

        var filterDefinition = Builders<Activity>.Filter.Eq(f => f.Name, "Activity I");
            //& Builders<Activity>.Filter.ElemMatch(f => f.Sessions, f => f.Name == "Session I");

        var updates = new List<UpdateDefinition<Activity>>
        {
            Builders<Activity>.Update.Set($"Sessions.$[].Exercises.$[].Challenges.$[j0].Name", "UPDATED"),
            Builders<Activity>.Update.Set($"Sessions.$[].Exercises.$[].Challenges.$[j1].Name", "UPDATED")
        };

        var updateDefinition = Builders<Activity>.Update.Combine(updates);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new BsonDocumentArrayFilterDefinition<SessionExercise>(new BsonDocument( "j0._id", new ObjectId(objectIds[0]))),
            new BsonDocumentArrayFilterDefinition<SessionExercise>(new BsonDocument( "j1._id", new ObjectId(objectIds[1]))),
        };

        var result = await collection.FindOneAndUpdateAsync(filterDefinition, updateDefinition,
            new FindOneAndUpdateOptions<Activity> { ReturnDocument = ReturnDocument.After, ArrayFilters = arrayFilters });
        Console.WriteLine(result.ToJson());

        await collection.DeleteManyAsync(_ => true);
    }
}
