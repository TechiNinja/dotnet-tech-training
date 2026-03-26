using System;
using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ISportRepository : IGenericRepository<Sport>
    {
    }
}