using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IRoleRepository: IGenericRepository<Role>
    {
        Task<Role?> GetRoleByTypeAsync(string RoleName);
    }
}
