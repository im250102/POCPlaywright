using backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var effectiveConnectionString = string.IsNullOrWhiteSpace(connectionString)
            ? "mongodb://127.0.0.1:27017"
            : connectionString;

        effectiveConnectionString = effectiveConnectionString
            .Replace("mongodb://localhost", "mongodb://127.0.0.1", StringComparison.OrdinalIgnoreCase)
            .Replace("mongodb://[::1]", "mongodb://127.0.0.1", StringComparison.OrdinalIgnoreCase);

        var client = new MongoClient(effectiveConnectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Patient> Patients =>
        _database.GetCollection<Patient>("Patients");

    public IMongoCollection<Appointment> Appointments =>
        _database.GetCollection<Appointment>("Appointments");

    public IMongoCollection<MedicalReport> MedicalReports =>
        _database.GetCollection<MedicalReport>("MedicalReports");
}
