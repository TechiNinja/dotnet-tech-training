using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class OperationsRepository : GenericRepository<EventRequest>, IOperationsRepository
{
    public OperationsRepository(AppDbContext context) : base(context) { }

}