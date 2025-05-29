using SecondWebApi.Interfaces;
using SecondWebApi.Misc;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class DoctorService : IDoctorService
{
    DoctorMapper doctorMapper;
    SpecialityMapper specialityMapper;
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialityRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    private readonly IOtherContextFunctionities _otherContextFunctionities;
    public DoctorService(
        IRepository<int, Doctor> doctorRepository,
        IRepository<int, Speciality> specialityRepository,
        IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
        IOtherContextFunctionities otherContextFunctionities)
    {
        doctorMapper = new DoctorMapper();
        specialityMapper = new SpecialityMapper();
        _doctorRepository = doctorRepository;
        _specialityRepository = specialityRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _otherContextFunctionities = otherContextFunctionities;
    }

    // public async Task<Doctor?> AddDoctor(DoctorAddDto doctorAddDto)
    // {
    //     try
    //     {
    //         if (doctorAddDto == null || string.IsNullOrWhiteSpace(doctorAddDto.Name))
    //             throw new ArgumentException("Invalid doctor details.");

    //         var allSpecialities = await _specialityRepository.GetAll();
    //         var specialityLookup = allSpecialities.ToDictionary(s => s.Name.ToLower(), s => s);

    //         var matchedSpecialities = new List<Speciality>();

    //         if (doctorAddDto.specialities != null && doctorAddDto.specialities.Any())
    //         {
    //             foreach (var specialityDto in doctorAddDto.specialities)
    //             {
    //                 if (specialityDto?.Name == null) continue;

    //                 var specialityNameLower = specialityDto.Name.ToLower();
    //                 if (specialityLookup.ContainsKey(specialityNameLower))
    //                 {
    //                     matchedSpecialities.Add(specialityLookup[specialityNameLower]);
    //                 }
    //                 else
    //                 {
    //                     throw new InvalidOperationException($"Speciality '{specialityDto.Name}' does not exist in the system.");
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             throw new InvalidOperationException("At least one speciality must be provided for the doctor.");
    //         }

    //         var doctor = new Doctor
    //         {
    //             Name = doctorAddDto.Name,
    //             YearsOfExperience = doctorAddDto.YearsOfExperience,
    //             Status = "Created"
    //         };

    //         var addedDoctor = await _doctorRepository.Add(doctor);
    //         if (addedDoctor == null)
    //             throw new InvalidOperationException("Failed to add doctor.");

    //         foreach (var speciality in matchedSpecialities)
    //         {
    //             var doctorSpeciality = new DoctorSpeciality
    //             {
    //                 DoctorId = addedDoctor.Id,
    //                 SpecialityId = speciality.Id
    //             };
    //             await _doctorSpecialityRepository.Add(doctorSpeciality);
    //         }

    //         return addedDoctor;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error Adding Doctor: {ex.Message}");
    //         return null;
    //     }
    // }

    public async Task<Doctor?> AddDoctor(DoctorAddDto doctorAddDto)
    {
        try
        {
            var addedDoctor = doctorMapper.MapDoctorAddRequest(doctorAddDto);
            addedDoctor = await _doctorRepository.Add(addedDoctor);
            if (addedDoctor == null)
                throw new Exception("Doctor cannot be added");
            if (doctorAddDto.specialities.Count() > 0)
            {
                int[] specialities = await MapAndAddSpeciality(doctorAddDto);
                for (int i = 0; i < specialities.Length; i++)
                {
                    var doctorSpeciality = specialityMapper.MapDoctorSpecialityAddRequest(addedDoctor.Id, specialities[i]);
                    doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
            }
            return addedDoctor;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private async Task<int[]> MapAndAddSpeciality(DoctorAddDto doctor)
    {
        int[] specialityIds = new int[doctor.specialities.Count()];
        IEnumerable<Speciality> existingSpecialities = null;
        try
        {
            existingSpecialities = await _specialityRepository.GetAll();
        }
        catch (Exception e)
        {

        }
        int count = 0;
        foreach (var item in doctor.specialities)
        {
            Speciality speciality = null;
            if (existingSpecialities != null)
                speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
            if (speciality == null)
            {
                speciality = specialityMapper.MapSpecialityAddRequest(item);
                speciality = await _specialityRepository.Add(speciality);
            }
            specialityIds[count] = speciality.Id;
            count++;
        }
        return specialityIds;
    }
    public async Task<Doctor?> GetDoctorByName(string name)
    {
        try
        {
            var allDoctors = await _doctorRepository.GetAll();
            var doctor = allDoctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
                throw new InvalidOperationException($"Doctor with name '{name}' not found.");



            return doctor;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Fetching Doctor By Name: {ex.Message}");
            return null;
        }
    }

    // public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string specialityName)
    // {
    //     try
    //     {
    //         var allDoctors = await _doctorRepository.GetAll();

    //         var doctors = allDoctors
    //             .Where(d => d.DoctorSpecialities != null &&
    //                         d.DoctorSpecialities.Any(ds => ds.Speciality != null &&
    //                                                        ds.Speciality.Name.Equals(specialityName, StringComparison.OrdinalIgnoreCase)))
    //             .ToList();

    //         if (!doctors.Any())
    //             throw new InvalidOperationException($"No doctors found for speciality '{specialityName}'.");

    //         return doctors;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error Fetching Doctors By Speciality: {ex.Message}");
    //         return new List<Doctor>();
    //     }
    // }
    public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
    {
        var result = await _otherContextFunctionities.GetDoctorsBySpeciality(speciality);
        return result;
    }


}
