using backend.Models;

namespace backend.Services;

public class DoctorService
{
    public static readonly string[] AllowedSpecialties =
    {
        "medicina-general",
        "cardiologia",
        "dermatologia",
        "endocrinologia",
        "gastroenterologia",
        "geriatria",
        "hematologia",
        "infectologia",
        "nefrologia",
        "neumologia",
        "neurologia",
        "oncologia",
        "oftalmologia",
        "otorrinolaringologia",
        "pediatria",
        "psiquiatria",
        "reumatologia",
        "urologia",
        "ginecologia",
        "obstetricia",
        "traumatologia",
        "anestesiologia",
        "radiologia",
        "medicina-interna"
    };

    private readonly IDoctorRepository _repo;

    public DoctorService(IDoctorRepository repo)
    {
        _repo = repo;
    }

    public async Task<DoctorResult<List<Doctor>>> GetDoctorsAsync()
    {
        var doctors = await _repo.GetDoctorsAsync();
        return DoctorResult<List<Doctor>>.Ok(doctors);
    }

    public async Task<DoctorResult<Doctor>> GetDoctorAsync(string id)
    {
        var doctor = await _repo.GetDoctorByIdAsync(id);
        if (doctor is null)
            return DoctorResult<Doctor>.NotFound();

        return DoctorResult<Doctor>.Ok(doctor);
    }

    public async Task<DoctorResult<Doctor>> CreateDoctorAsync(DoctorRequest request, string userId)
    {
        var (valid, error) = Validate(request);
        if (!valid)
            return DoctorResult<Doctor>.Invalid(error);

        var doctor = new Doctor
        {
            UserId = userId,
            Name = request.Name.Trim(),
            LastName = request.LastName.Trim(),
            Phone = request.Phone.Trim(),
            Specialty = NormalizeSpecialty(request.Specialty)
        };

        await _repo.CreateDoctorAsync(doctor);
        return DoctorResult<Doctor>.Created(doctor);
    }

    public async Task<DoctorResult<Doctor>> UpdateDoctorAsync(string id, DoctorRequest request)
    {
        var existing = await _repo.GetDoctorByIdAsync(id);
        if (existing is null)
            return DoctorResult<Doctor>.NotFound();

        var (valid, error) = Validate(request);
        if (!valid)
            return DoctorResult<Doctor>.Invalid(error);

        existing.Name = request.Name.Trim();
        existing.LastName = request.LastName.Trim();
        existing.Phone = request.Phone.Trim();
        existing.Specialty = NormalizeSpecialty(request.Specialty);

        await _repo.UpdateDoctorAsync(existing);
        return DoctorResult<Doctor>.Ok(existing);
    }

    public async Task<DoctorResult> DeleteDoctorAsync(string id)
    {
        var existing = await _repo.GetDoctorByIdAsync(id);
        if (existing is null)
            return DoctorResult.NotFound();

        var deleted = await _repo.DeleteDoctorAsync(id);
        return deleted
            ? DoctorResult.Ok()
            : DoctorResult.NotFound();
    }

    public (bool Valid, string Error) Validate(DoctorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "El nombre es obligatorio");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return (false, "Los apellidos son obligatorios");

        if (string.IsNullOrWhiteSpace(request.Phone))
            return (false, "El teléfono es obligatorio");

        if (!IsValidSpecialty(request.Specialty))
            return (false, "La especialidad no es válida");

        return (true, string.Empty);
    }

    public static bool IsValidSpecialty(string specialty) =>
        AllowedSpecialties.Any(s => string.Equals(s, specialty, StringComparison.OrdinalIgnoreCase));

    public static string NormalizeSpecialty(string specialty) =>
        AllowedSpecialties.First(s => string.Equals(s, specialty, StringComparison.OrdinalIgnoreCase));
}