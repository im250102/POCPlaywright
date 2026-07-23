using backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Patient> Patients =>
        _database.GetCollection<Patient>("Patients");

    public IMongoCollection<Appointment> Appointments =>
        _database.GetCollection<Appointment>("Appointments");

    public IMongoCollection<MedicalReport> MedicalReports =>
        _database.GetCollection<MedicalReport>("MedicalReports");
}
