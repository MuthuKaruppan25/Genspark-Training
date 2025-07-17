using JobPortal.Models;

public class RequirementMapper
{
    public List<Requirements> MapList(ICollection<RequirementsAddDto> requirementNames, Guid postId)
    {
        return requirementNames
            .Where(req => !string.IsNullOrWhiteSpace(req.Name))
            .Select(req => new Requirements
            {
                guid = Guid.NewGuid(),
                Name = req.Name.Trim(),
                PostId = postId,

            })
            .ToList();
    }
    
    public List<Requirements> MapUpdateList(RequirementsUpdateDto requirementNames, Guid postId)
    {
        return requirementNames.UpdatedRequirements!
            .Where(req => !string.IsNullOrWhiteSpace(req.Name))
            .Select(req => new Requirements
            {
                guid = Guid.NewGuid(),
                Name = req.Name.Trim(),
                PostId = postId,
                
            })
            .ToList();
    }
}
