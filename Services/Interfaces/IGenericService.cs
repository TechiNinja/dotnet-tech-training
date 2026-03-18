namespace SportsManagementApp.Services.Interfaces
{
    public interface IGenericService<TResponseDto>
    {
        Task<TResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<TResponseDto>> GetAllAsync();
    }
}