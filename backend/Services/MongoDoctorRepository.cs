using backend.Models;
using MongoDB.Driver;

namespace backend.Services;

public class MongoDoctorRepository : IDoctorRepository
{
    private readonly MongoDbContext _db;

    public MongoDoctorRepository(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<List<Doctor>> GetDoctorsAsync() =>
        await _db.Doctors.Find(_ => true).ToListAsync();

    public async Task<Doctor?> GetDoctorByIdAsync(string id) =>
        await _db.Doctors.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        await _db.Doctors.InsertOneAsync(doctor);
        return doctor;
    }

    public async Task<bool> UpdateDoctorAsync(Doctor doctor)
    {
        var result = await _db.Doctors.ReplaceOneAsync(d => d.Id == doctor.Id, doctor);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteDoctorAsync(string id)
    {
        var deleted = await _db.Doctors.DeleteOneAsync(d => d.Id == id);
        return deleted.IsAcknowledged && deleted.DeletedCount > 0;
    }
}