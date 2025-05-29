using BankApi.Models;

namespace BankApi.Interfaces;

public interface IBranchService
{
    Task<Branch> AddBranch(Branch branch);
    Task<IEnumerable<Branch>> GetAllBranches();
    Task<Branch?> GetBranchById(int branchId);
}
