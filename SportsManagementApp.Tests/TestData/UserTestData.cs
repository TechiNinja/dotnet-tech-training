using System;
using System.Collections.Generic;
using System.Text;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Tests.TestData
{
    public static class UserTestData
    {
        public static List<User> UserEntityList() => new()
        {
            new() { Id = 2, FullName = "Vishal Kumar", Email = "vishal@test.com", RoleId = 2 },
            new() { Id = 5, FullName = "Navneet Kumar", Email = "navneet@test.com", RoleId = 3 }
        };

        public static User UserHimani() => new()
            { Id = 4, FullName = "Himani Jangid", Email = "himani.jangid@intimetec.com", RoleId = 1 };

        public static User UserVishalWithOldHash() => new()
            { Id = 2, FullName = "Vishal Kumar", Email = "vishal@test.com", PasswordHash = "oldhash", RoleId = 2 };

        public static User UserNavneetWithOldHash() => new()
            { Id = 5, FullName = "Navneet Kumar", Email = "navneet@test.com", PasswordHash = "oldhash", RoleId = 3 };

        public static User UserVaishak() => new()
            { Id = 6, FullName = "Vaishak", Email = "vaishak@test.com", PasswordHash = "hash", RoleId = 2 };

        public static User MappedNewUser() => new()
            { FullName = "Himesh Kumar", Email = "himesh@test.com" };

        public static List<UserResponseDto> UserResponseList() => new()
        {
            new() { Id = 2, FullName = "Vishal Kumar", Email = "vishal@test.com", RoleId = 2 },
            new() { Id = 5, FullName = "Navneet Kumar", Email = "navneet@test.com", RoleId = 3 }
        };

        public static UserResponseDto UserHimaniResponse() => new()
            { Id = 4, FullName = "Himani Jangid", Email = "himani.jangid@intimetec.com", RoleId = 1 };

        public static UserResponseDto CreatedHimeshResponse() => new()
            { Id = 22, FullName = "Himesh Kumar", Email = "himesh@test.com" };

        public static UserResponseDto UpdatedVishalResponse() => new()
            { Id = 2, FullName = "Vishal Kumar Jangid", Email = "vishal@test.com" };

        public static UserResponseDto UpdatedNavneetResponse() => new()
            { Id = 5, FullName = "Navneet Kumar Jaiswal", Email = "navneet@test.com" };

        public static UserResponseDto UpdatedVaishakResponse() => new()
            { Id = 6, FullName = "Vaishak S", Email = "vaishak@test.com", IsActive = false };

        public static CreateUserDto DuplicateEmailDto() => new()
            { FullName = "Vishal Kumar", Email = "vishal@test.com", Password = "Pass@12345", RoleId = 2 };

        public static CreateUserDto ValidCreateDto() => new()
            { FullName = "Himesh Kumar", Email = "himesh@test.com", Password = "Test@1234", RoleId = 4 };

        public static UpdateUserDto UpdateVishalWithPassword() => new()
            { FullName = "Vishal Kumar Jangid", Password = "NewPass@123" };

        public static UpdateUserDto UpdateNavneetWithoutPassword() => new()
            { FullName = "Navneet Kumar Jaiswal", Password = "" };

        public static UpdateUserDto UpdateVaishak() => new()
            { FullName = "Vaishak S", IsActive = false };
    }
}
