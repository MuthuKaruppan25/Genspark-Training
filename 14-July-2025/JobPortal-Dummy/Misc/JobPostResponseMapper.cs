using JobPortal.Models;
using JobPortal.Models.DTOs;

public class JobPostResponseMapper
{
    public JobPostRegisterResponseDto Map(JobPost jobPost)
    {
        return new JobPostRegisterResponseDto
        {
            PostId = jobPost.guid,
            Title = jobPost.Title,
            RecruiterId = jobPost.RecruiterID,
            PostedDate = jobPost.PostedDate,
            LastDate = jobPost.LastDate
        };
    }
}
