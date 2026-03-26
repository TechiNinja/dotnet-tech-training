using System;
using System.Linq.Expressions;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SportRepository : GenericRepository<Sport>, ISportRepository
    {
        public SportRepository(AppDbContext context) : base(context)
        {
        }

    }
}