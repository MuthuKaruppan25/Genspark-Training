
using JobPortal.Models.DTOs;

public interface ITransactionalRecruiterRegister
{
    Task<RecruiterRegisterResponseDto> RegisterCompany(RecruiterRegisterDto dto);
}