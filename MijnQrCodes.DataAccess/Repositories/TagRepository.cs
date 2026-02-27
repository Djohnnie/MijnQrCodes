using Microsoft.EntityFrameworkCore;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public class TagRepository : ITagRepository
{
    private readonly MijnQrCodesDbContext _context;

    public TagRepository(MijnQrCodesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetAll()
    {
        return await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetById(Guid id)
    {
        return await _context.Tags
            .SingleOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tag> Create(Tag tag)
    {
        tag.Id = Guid.NewGuid();
        tag.CreatedAt = DateTime.UtcNow;
        tag.UpdatedAt = DateTime.UtcNow;
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag?> Update(Tag tag)
    {
        var existing = await GetById(tag.Id);
        if (existing == null) return null;

        existing.Name = tag.Name;
        existing.Color = tag.Color;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> Delete(Guid id)
    {
        var deleted = await _context.Tags
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }
}
