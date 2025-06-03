

using System.Threading.Tasks;
using AutoMapper;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _repository;
    private readonly IRepository<string, User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IEncryptionService _encryptionService;
    PatientAddReqMapper patientAddReqMapper;

    public PatientService(IRepository<int, Patient> repository, IMapper mapper, IRepository<String, User> userRepository, IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        patientAddReqMapper = new PatientAddReqMapper();
        _mapper = mapper;
        _encryptionService = encryptionService;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null");
    }

    public async Task<Patient?> AddPatientAsync(PatientAddDto patientAddDto)
    {
        try
        {

            var user = _mapper.Map<PatientAddDto, User>(patientAddDto);
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = patientAddDto.Password
            });
            user.password = encryptedData.EncryptedData;
            user.HashKey = encryptedData.HashKey;
            user.role = "Doctor";
            user = await _userRepository.Add(user);
            if (patientAddDto == null)
            {
                throw new ArgumentNullException("Patient cannot be null");
            }

            var patient = patientAddReqMapper.MapPatientAddRequest(patientAddDto);
            var addedPatient = await _repository.Add(patient!);
            return addedPatient;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding patient: {ex.Message}");
            return null;
        }
    }
    public async Task DeletePatient(int id)
    {
        try
        {

            await _repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient with ID {id}: {ex.Message}");
        }
    }
    public async Task<Patient?> UpdatePatient(int id, Patient patient)
    {
        try
        {
            return await _repository.Update(id, patient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient with ID {id}: {ex.Message}");
            return null;
        }
    }
    public async Task<IEnumerable<Patient>?> GetPatientByName(string name)
    {
        try
        {
            var patients = await _repository.GetAll();
            if (patients.Count() == 0)
            {
                throw new Exception("No Patients Found");
            }
            var FilteredPatients = patients.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return FilteredPatients;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving patient with name : {name}: {ex.Message}");
            return null;
        }
    }
}