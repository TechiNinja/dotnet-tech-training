using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IParticipantRegistrationRepository: IGenericRepository<ParticipantRegistration>
    {
        Task<bool> IsUserRegisteredInCategoryAsync(int userId, int categoryId);
        Task<List<ParticipantRegistration>> GetParticipantsByCategoryAsync(int categoryId);
        Task<ParticipantRegistration?> GetParticipantsByIdWithUserAsync(int id);
    }
}
