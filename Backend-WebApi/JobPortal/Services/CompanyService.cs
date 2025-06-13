using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using log4net;

public class CompanyService : ICompanyService
{
    private readonly IRepository<Guid, Company> _repository;
    private readonly IRepository<Guid, IndustryType> _industryTypeRepository;
    private readonly IRepository<Guid, Address> _addressRepository;

    private readonly IndustryTypeMapper _industryMapper;
    private readonly CompanyMapper _companyMapper;
    private readonly AddressMapper _addressMapper;
    private readonly CompanyRegisterResponseMapper _responseMapper;
    private readonly IRepository<Guid, Recruiter> _recruiterRepository;
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public CompanyService(IRepository<Guid, Company> repository, IRepository<Guid, IndustryType> industryTypeRepository, IRepository<Guid, Address> addressRepository,IRepository<Guid, Recruiter> recruiterRepository)
    {
        _industryMapper = new IndustryTypeMapper();
        _companyMapper = new CompanyMapper();
        _addressMapper = new AddressMapper();
        _responseMapper = new CompanyRegisterResponseMapper();
        _recruiterRepository = recruiterRepository;
        _industryTypeRepository = industryTypeRepository;
        _addressRepository = addressRepository;
        _repository = repository;
    }

    public async Task<CompanyRegisterResponseDto> RegisterCompany(CompanyRegisterDto companyRegisterDto)
    {
        try
        {
            var existingCompanies = await _repository.GetAll();
            if (existingCompanies.Any(c =>
                    string.Equals(c.CompanyName, companyRegisterDto.CompanyName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateEntryException($"The company with name {companyRegisterDto.CompanyName} already exists.");
            }

            if (companyRegisterDto.industryTypeRegister == null || string.IsNullOrWhiteSpace(companyRegisterDto.industryTypeRegister.Name))
            {
                throw new FieldRequiredException("Industry type field is required.");
            }

            var existingIndustries = await _industryTypeRepository.GetAll();
            var matchedIndustry = existingIndustries.FirstOrDefault(i =>
                i.Name.Equals(companyRegisterDto.industryTypeRegister.Name, StringComparison.OrdinalIgnoreCase));

            IndustryType industry;
            if (matchedIndustry != null)
            {
                industry = matchedIndustry;
            }

            else
            {
                throw new RecordNotFoundException($"Industry type '{companyRegisterDto.industryTypeRegister.Name}' not found and could not be created.");
            }

            var company = _companyMapper.MapCompany(companyRegisterDto, industry);
            await _repository.Add(company);

            var addresses = _addressMapper.MapAddresses(companyRegisterDto.locations?.ToList(), company.guid);
            foreach (var address in addresses)
            {
                address.guid = Guid.NewGuid();
                await _addressRepository.Add(address);
            }

            _dataLogger.Info($"Company registered: Name = {company.CompanyName}, CompanyId = {company.guid}");

            return _responseMapper.MapResponse(company);
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field validation failed during company registration", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Duplicate company name found during registration", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Industry type error during company registration", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Company registration failed due to an unexpected error", ex);
            throw new RegistrationException("Company registration failed due to an unexpected error.", ex);
        }
    }


    public async Task<IEnumerable<Recruiter>> GetRecruitersInCompany(Guid companyId)
    {
        try
        {
            var companies = await _repository.GetAll();
            var company = companies.FirstOrDefault(c => c.guid == companyId);
            if (company == null)
                throw new RecordNotFoundException("Company not found.");


            var recruiters = company.recruiters?.Where(r => !r.IsDeleted).ToList();
            return recruiters ?? new List<Recruiter>();
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch recruiters for company", ex);
            throw new FetchDataException("Failed to fetch recruiters for company", ex);
        }
    }

    public async Task<IEnumerable<Address>> GetCompanyLocations(Guid companyId)
    {
        try
        {
            var addresses = await _addressRepository.GetAll();
            var companyLocations = addresses.Where(a => a.companyId == companyId).ToList();
            return companyLocations;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch company locations", ex);
            throw new FetchDataException("Failed to fetch company locations", ex);
        }
    }

    public async Task UpdateCompanyDetails(Guid companyId, CompanyUpdateDto updateDto)
    {
        try
        {
            var company = await _repository.Get(companyId);
            if (company == null)
                throw new RecordNotFoundException("Company not found.");

            company.CompanyName = updateDto.CompanyName;
            company.Description = updateDto.Description;
            company.WebsiteUrl = updateDto.WebsiteUrl;


            await _repository.Update(companyId, company);
            _dataLogger.Info($"Company details updated: Id={companyId}");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Company not found for update", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to update company details", ex);
            throw new UpdateException("Failed to update company details", ex);
        }
    }

    public async Task UpdateCompanyLocations(Guid companyId, List<AddressRegisterDto> locationDtos)
    {
        try
        {
            var company = await _repository.Get(companyId);
            if (company == null)
                throw new RecordNotFoundException("Company not found.");

            var existingAddresses = (await _addressRepository.GetAll()).Where(a => a.companyId == companyId).ToList();

            // Remove old addresses
            foreach (var address in existingAddresses)
            {
                await _addressRepository.Delete(address.guid);
            }

            // Add new addresses
            var newAddresses = _addressMapper.MapAddresses(locationDtos, companyId);
            foreach (var address in newAddresses)
            {
                address.guid = Guid.NewGuid();
                await _addressRepository.Add(address);
            }

            _dataLogger.Info($"Company locations updated: Id={companyId}");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Company not found for updating locations", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to update company locations", ex);
            throw new UpdateException("Failed to update company locations", ex);
        }

    }
    public async Task<Company> SoftDeleteCompany(Guid companyId)
    {
        try
        {
            var company = await _repository.Get(companyId);
            if (company == null)
                throw new RecordNotFoundException("Company not found.");

            company.IsDeleted = true;
            await _repository.Update(companyId, company);

            var recruiters = await _recruiterRepository.GetAll();
            foreach (var recruiter in recruiters.Where(r => r.CompanyId == companyId && !r.IsDeleted))
            {
                recruiter.IsDeleted = true;
                await _recruiterRepository.Update(recruiter.guid, recruiter);
            }

            _dataLogger.Info($"Company soft-deleted: Id={companyId}");
            return company;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Company not found for soft delete", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to soft delete company", ex);
            throw new UpdateException("Failed to soft delete company", ex);
        }
    }
}