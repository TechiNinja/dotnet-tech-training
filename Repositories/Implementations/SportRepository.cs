﻿using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data.DTOs.SportManagement;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SportRepository : GenericRepository<Sport>, ISportRepository
    {
        public SportRepository(AppDbContext context) : base(context) { }

        public async Task<bool> SportExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(sport => sport.Name == name);
        }

        public async Task<Sport> CreateSportAsync(string name, List<string> allowedFormats)
        {
            var sport = new Sport
            {
                Name = name,
                AllowedFormats = allowedFormats,
                CreatedAt = DateTime.UtcNow
            };

            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            return sport;
        }

        public async Task<List<Sport>> GetSportsAsync(Expression<Func<Sport, bool>> predicate)
        {
            return await _context.Sports
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Sport?> GetSportByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task UpdateSportAsync(Sport sport)
        {
            await UpdateAsync(sport);
        }
    }
}
