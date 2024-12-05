using BHF.MS.new8.Database.Models.DbItem;

namespace BHF.MS.new8.Database.Services
{
    public interface IDbItemService
    {
        public Task<IList<DbItemDto>> GetAll();

        public Task<DbItemDto?> GetById(Guid id);

        public Task<bool> Update(DbItemDto model);

        public Task<DbItemDto> Add(DbItemCreateDto model);

        public Task<bool> Delete(Guid id);
    }
}

