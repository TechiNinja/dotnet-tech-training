using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryService : IGenericService<EventCategory>
    {
        Task<FixtureResponseDto> GetMatchByIdAsync(int matchId);
    }
}