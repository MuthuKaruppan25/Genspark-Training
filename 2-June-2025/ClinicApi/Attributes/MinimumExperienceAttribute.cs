using Microsoft.AspNetCore.Authorization;

public class MinimumExperienceAuthorizeAttribute : AuthorizeAttribute
{
    public MinimumExperienceAuthorizeAttribute(int years)
    {
        Policy = $"DoctorWithExperience{years}";
    }
}
