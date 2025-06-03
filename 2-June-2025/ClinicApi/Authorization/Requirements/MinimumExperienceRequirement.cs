using Microsoft.AspNetCore.Authorization;

public class MinimumExperienceRequirement : IAuthorizationRequirement
{
    public int MinimumYears { get; set; }


}