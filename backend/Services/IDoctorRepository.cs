using backend.Models;

namespace backend.Services;

public interface IDoctorRepository
{
    Task<List<Doctor>> GetDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(string id);
    Task<Doctor> CreateDoctorAsync(Doctor doctor);
    Task<bool> UpdateDoctorAsync(Doctor doctor);
    Task<bool> DeleteDoctorAsync(string id);
}