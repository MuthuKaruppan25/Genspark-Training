using BankApi.Models;
using BankApi.Interfaces;
namespace BankApi.Services;
public class BranchService : IBranchService
{
    private readonly IRepository<string, Branch> _repository;

    public BranchService(IRepository<string, Branch> repository)
    {
        _repository = repository;
    }

    public async Task<Branch> AddBranch(Branch branch)
    {
        try
        {
            var addedBranch = await _repository.Add(branch);
            if (addedBranch == null)
                throw new InvalidOperationException("Failed to add branch.");
            return addedBranch;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddBranch: {ex.Message}");
            throw; // rethrow to propagate error to controller
        }
    }

    public async Task<IEnumerable<Branch>> GetAllBranches()
    {
        try
        {
            var branches = await _repository.GetAll();
            return branches;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllBranches: {ex.Message}");
            throw;
        }
    }

    public async Task<Branch?> GetBranchById(int branchId)
    {
        try
        {
            var branch = await _repository.Get(branchId.ToString());
            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {branchId} not found.");
            return branch;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetBranchById: {ex.Message}");
            throw;
        }
    }
}
