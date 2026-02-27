using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public interface ITagRepository
{
    Task<List<Tag>> GetAll();
    Task<Tag?> GetById(Guid id);
    Task<Tag> Create(Tag tag);
    Task<Tag?> Update(Tag tag);
    Task<bool> Delete(Guid id);
}
