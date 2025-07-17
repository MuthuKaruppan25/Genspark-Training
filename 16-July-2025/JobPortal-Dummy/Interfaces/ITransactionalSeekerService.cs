using JobPortal.Models.DTOs;

public interface ITransactionalSeekerService
{
    Task<SeekerRegisterResponseDto> RegisterSeekerWithTransaction(SeekerRegisterDto dto);
}