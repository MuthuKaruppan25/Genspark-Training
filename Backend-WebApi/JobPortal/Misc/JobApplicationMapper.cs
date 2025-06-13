using JobPortal.Models;

public class JobApplicationMapper
{
    public JobApplication? Map(JobApplicantAddDto dto, Guid seekerId)
    {
        return new JobApplication
        {
            guid = Guid.NewGuid(),
            AppliedOn = DateTime.UtcNow,
            Status = "Applied",
            IsDeleted = false,
            JobPostId = dto.JobPostId,
            SeekerId = seekerId
        };
    }

    public JobApplicantAddResponse? MapToResponse(JobApplication entity)
    {
        return new JobApplicantAddResponse
        {
            AppliedOn = entity.AppliedOn,
            Status = entity.Status,
            JobPostId = entity.JobPostId,
            SeekerId = entity.SeekerId
        };
    }
}
