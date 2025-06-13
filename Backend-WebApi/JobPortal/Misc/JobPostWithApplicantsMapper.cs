using System;
using System.Linq;
using System.Collections.Generic;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class JobPostWithApplicantsMapper
{
    public JobPostWithApplicantsDto Map(JobPost post, List<JobApplicantAddResponse> pagedApplicants, int totalCount)
    {
        return new JobPostWithApplicantsDto
        {
            JobPostId = post.guid,
            Title = post.Title,
            Description = post.Description,
            RequiredSkills = post.requiredSkills?.Select(rs => rs.Skill.Name).ToList() ?? new List<string>(),
            Applicants = new PagedResult<JobApplicantAddResponse>
            {
                Items = pagedApplicants,
                TotalCount = totalCount
            }
        };
    }
}