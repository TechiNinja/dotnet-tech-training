﻿using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _sportRepository;

        public SportService(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        public async Task<Sport> CreateSportAsync(CreateSportDto createSport)
        {
            if (string.IsNullOrWhiteSpace(createSport.Name))
            {
                throw new BadRequestException("Sport Name is required");
            }

            var exists = await _sportRepository.SportExistsAsync(createSport.Name.Trim());

            if (exists)
            {
                throw new ConflictException("Sport already exists");
            }

            return await _sportRepository.CreateSportAsync(createSport.Name.Trim());
        }

        public async Task<List<SportResponseDto>> GetSportsAsync(SportFilterDto filter)
        {
            return await _sportRepository.GetSportsAsync(
                SportPredicateBuilder.Build(filter),
                SportProjectionBuilder.Build()
            );
        }

        public async Task<Sport> UpdateSportAsync(int id, UpdateSportDto updateSport)
        {
            if (string.IsNullOrEmpty(updateSport.Name))
            {
                throw new BadRequestException("Sport name is required");
            }

            var sport = await _sportRepository.GetSportByIdAsync(id);

            if (sport == null)
            {
                throw new NotFoundException("Sport not found");
            }

            var exists = await _sportRepository.SportExistsAsync(updateSport.Name.Trim());

            if (exists && !sport.Name.Equals(updateSport.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException("Sport with this name already exists");
            }

            sport.Name = updateSport.Name.Trim();
            sport.UpdatedAt = DateTime.UtcNow;

            await _sportRepository.UpdateSportAsync(sport);
            return sport;
        }
    }
}
