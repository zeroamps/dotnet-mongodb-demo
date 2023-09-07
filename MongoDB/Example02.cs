using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Example02;

internal class Device
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public IList<int> Measurements { get; set; }
    public IList<DateTime> Failures { get; set; }

    public Device()
    {
        this.Measurements = new List<int>();
        this.Failures = new List<DateTime>();
    }
}

internal class Main
{
    public async static Task Run(IMongoDatabase database)
    {
        var collection = database.GetCollection<Device>("devices");

        var documents = new[] {
            new Device {
                Name = "Temperature",
                Measurements = { -10, -5, -3, 0, 1, 3, 7, 10, 13, 17, 23, 28, 36 },
                Failures = { new DateTime(2000, 1, 1), new DateTime(2008, 1, 1), new DateTime(2022, 1, 1) }
            },
            new Device  {
                Name = "Humidity",
                Measurements = { 10, 40, 30, 80, 60, 35, 70, 90, 15, 25, 60 },
                Failures = { new DateTime(2000, 1, 1), new DateTime(2008, 1, 1), new DateTime(2022, 1, 1) }
            }
        };

        //await collection.InsertManyAsync(documents);

        Console.WriteLine(await collection.CountDocumentsAsync(new BsonDocument { }));
        Console.WriteLine(await collection.CountDocumentsAsync(Builders<Device>.Filter.Empty));
        Console.WriteLine();

        Console.WriteLine((await collection.Find(new BsonDocument { }).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(Builders<Device>.Filter.Empty).ToListAsync()).ToJson());
        Console.WriteLine();

        var filter = Builders<Device>.Filter.Eq(f => f.Name, "Temperature");
        Console.WriteLine((await collection.Find(filter).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(f => f.Name == "Temperature").ToListAsync()).ToJson());
        Console.WriteLine();

        var builder = Builders<Device>.Filter;
        filter = builder.Eq(f => f.Name, "Temperature") & builder.AnyEq(f => f.Measurements, 10);
        Console.WriteLine((await collection.Find(filter).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(f => f.Name == "Temperature" && f.Measurements.Any(e => e == 10)).ToListAsync()).ToJson());
        Console.WriteLine();

        var sort = Builders<Device>.Sort.Ascending(f => f.Name);
        Console.WriteLine((await collection.Find(new BsonDocument { }).Sort(sort).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(new BsonDocument { }).SortBy(f => f.Name).ToListAsync()).ToJson());
        Console.WriteLine();

        var projection = Builders<Device>.Projection.Expression(f => new Device { Id = f.Id, Name = f.Name, Measurements = f.Measurements });
        Console.WriteLine((await collection.Find(new BsonDocument { }).Project(projection).ToListAsync()).ToJson());

        projection = Builders<Device>.Projection.Expression(f => new Device { Id = f.Id, Name = f.Name, Measurements = f.Measurements.Where(e => e > 10).ToList() });
        Console.WriteLine((await collection.Find(new BsonDocument { }).Project(projection).ToListAsync()).ToJson());

        projection = Builders<Device>.Projection.Include(f => f.Id).Include(f => f.Name).Include(f => f.Measurements);
        Console.WriteLine((await collection.Find(new BsonDocument { }).Project(projection).ToListAsync()).ToJson());
        Console.WriteLine((await collection.Find(new BsonDocument { }).Project(
            p => new Device { Id = p.Id, Name = p.Name, Measurements = p.Measurements.Where(e => e > 10).ToList() }).ToListAsync()).ToJson());
        Console.WriteLine();

        Console.WriteLine(
            (await collection
            .Find(new BsonDocument { })
            .Project(
                p => new Device
                {
                    Id = p.Id,
                    Name = p.Name,
                    Measurements = p.Measurements.Where(e => e > 10).ToList(),
                    Failures = p.Failures.Where(e => e < DateTime.Now).ToList()
                })
            .ToListAsync()).ToJson());
        Console.WriteLine();
    }
}
