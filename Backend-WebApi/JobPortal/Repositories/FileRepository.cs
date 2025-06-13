



using JobPortal.Contexts;
using JobPortal.Repositories;
using Microsoft.EntityFrameworkCore;



namespace JobPortal.Repositories;
public class FileRepository : Repository<Guid, FileModel>
{
    public FileRepository(JobContext context) : base(context)
    {

    }
    public override async Task<FileModel> Get(Guid key)
    {
        var file = await _jobContext.fileModels.SingleOrDefaultAsync(u => u.guid == key);
        if (file is null)
        {
            throw new Exception("No such user found");
        }
        return file;
    }

    public override async Task<IEnumerable<FileModel>> GetAll()
    {
        return await _jobContext.fileModels.ToListAsync();
    }

}