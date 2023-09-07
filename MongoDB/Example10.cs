using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.Example10;

internal class Company
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public IList<string> Departments { get; set; }

    public IList<Department> DepartmentList { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public IList<string> Buildings { get; set; }

    public IList<Building> BuildingList { get; set; }

    public Company()
    {
        Departments = new List<string>();
        Buildings = new List<string>();
    }
}

internal class Department
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
}

internal class Building
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
        var departmentsCollection = database.GetCollection<Department>("departments");
        var buildingsCollection = database.GetCollection<Building>("buildings");
        var companiesCollection = database.GetCollection<Company>("companies");

        await departmentsCollection.InsertManyAsync(new[]
        {
            new Department { Name = "Department 1" },
            new Department { Name = "Department 2" },
            new Department { Name = "Department 3" },
            new Department { Name = "Department 4" },
            new Department { Name = "Department 5" }
        });

        await buildingsCollection.InsertManyAsync(new[]
        {
            new Building { Name = "Building 1" },
            new Building { Name = "Building 2" },
            new Building { Name = "Building 3" },
            new Building { Name = "Building 4" },
            new Building { Name = "Building 5" }
        });

        var company = new Company { Name = "Company 1" };
        var departments = await departmentsCollection.Find(_ => true).ToListAsync();
        foreach (var department in departments)
        {
            company.Departments.Add(department.Id);
        }

        var buildings = await buildingsCollection.Find(_ => true).ToListAsync();
        foreach (var building in buildings)
        {
            company.Buildings.Add(building.Id);
        }
        await companiesCollection.InsertOneAsync(company);

        // Approach 1

        var companies = await companiesCollection
            .Aggregate()
            .Match(f => f.Name == "Company 1")
            .Lookup<Company, Department, Company>(departmentsCollection, f => f.Departments, f => f.Id, f => f.DepartmentList)
            .Lookup<Company, Building, Company>(buildingsCollection, f => f.Buildings, f => f.Id, f => f.BuildingList)
            .ToListAsync();

        // Approach 2

        var pipeline = new EmptyPipelineDefinition<Company>()
            .AppendStage(PipelineStageDefinitionBuilder.Match<Company>(f => f.Name == "Company 1"))
            .AppendStage(PipelineStageDefinitionBuilder.Lookup<Company, Department, Company>(
                departmentsCollection, f => f.Departments, f => f.Id, f => f.DepartmentList))
            .AppendStage(PipelineStageDefinitionBuilder.Lookup<Company, Building, Company>(
                buildingsCollection, f => f.Buildings, f => f.Id, f => f.BuildingList));
        companies = await companiesCollection.Aggregate(pipeline).ToListAsync();

        await companiesCollection.DeleteManyAsync(_ => true);
        await departmentsCollection.DeleteManyAsync(_ => true);
        await buildingsCollection.DeleteManyAsync(_ => true);
    }
}
