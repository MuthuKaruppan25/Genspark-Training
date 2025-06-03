
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
    {
        if (!context.User.IsInRole("Doctor"))
        {
            return Task.CompletedTask;
        }
        var experienceClaim = context.User.FindFirst("YearsOfExperience");
        if (experienceClaim == null)
        {
            return Task.CompletedTask;
        }

        if (int.TryParse(experienceClaim.Value, out int experienceYears) && experienceYears >= requirement.MinimumYears)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}