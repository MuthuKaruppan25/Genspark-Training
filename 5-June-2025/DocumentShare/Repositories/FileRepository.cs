


using DocumentShare.Contexts;
using DocumentShare.Models;
using Microsoft.EntityFrameworkCore;



namespace DocumentShare.Repositories;
public class FileRepository : Repository<Guid, FileModel>
{
    public FileRepository(FileContext context) : base(context)
    {

    }
    public override async Task<FileModel> Get(Guid key)
    {
        var file = await _clinicContext.Files.SingleOrDefaultAsync(u => u.guid == key);
        if (file is null)
        {
            throw new Exception("No such user found");
        }
        return file;
    }

    public override async Task<IEnumerable<FileModel>> GetAll()
    {
        return await _clinicContext.Files.ToListAsync();
    }

}