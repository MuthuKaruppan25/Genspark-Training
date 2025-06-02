

namespace BankApi.Models;

public class Branch
{
    public string IFSCCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ICollection<Account>? Accounts { get; set; }

}