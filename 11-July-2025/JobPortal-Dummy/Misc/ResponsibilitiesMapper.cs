using JobPortal.Models;

public class ResponsibilityMapper
{
    public List<Responsibilities> MapList(ICollection<ResponsibilitiesAddDto> responsibilityNames, Guid postId)
    {
        return responsibilityNames
            .Where(res => !string.IsNullOrWhiteSpace(res.Name))
            .Select(res => new Responsibilities
            {
                guid = Guid.NewGuid(),
                Name = res.Name.Trim(),
                PostId = postId,

            })
            .ToList();
    }
    public List<Responsibilities> MapUpdateList(ResponsibilitiesUpdateDto responsibilityDto, Guid postId)
{
    return responsibilityDto.UpdatedResponsibilities!
        .Where(res => !string.IsNullOrWhiteSpace(res.Name))
        .Select(res => new Responsibilities
        {
            guid = Guid.NewGuid(),
            Name = res.Name.Trim(),
            PostId = postId,
        })
        .ToList();
}

}
