using Microsoft.AspNetCore.Identity;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class AuthTestData
    {
        public static readonly PasswordHasher<User> _hasher = new();

        public static LoginRequestDto ValidLoginRequest() => new()
        {
            Email = "himani.jangid@intimetec.com",
            Password = "Test@1234"
        };

        public static LoginRequestDto NonExistentEmailRequest() => new()
        {
            Email = "nonexistent@test.com",
            Password = "Pass@1234"
        };

        public static LoginRequestDto WrongPasswordRequest() => new()
        {
            Email = "vishal@test.com",
            Password = "wrongPass@123"
        };

        public static LoginRequestDto DeactivatedUserLoginRequest() => new()
        {
            Email = "navneet@test.com",
            Password = "Pass@123"
        };

        public static LoginRequestDto NullRoleUserLoginRequest() => new()
        {
            Email = "vaishak@test.com",
            Password = "Pass@123"
        };

        public static RegisterRequestDto ExistingEmailRegisterRequest() => new()
        {
            FullName = "Vishal Kumar",
            Email = "vishal@test.com",
            Password = "Test@1234"
        };

        public static RegisterRequestDto NewUserRegisterRequest() => new()
        {
            FullName = "New User",
            Email = "newuser@test.com",
            Password = "Test@1234"
        };

        public static RegisterRequestDto ValidRgisterRequest() => new()
        {
            FullName = "Piyush",
            Email = "piyush@test.com",
            Password = "Test@1234"
        };

        public static User ActiveAdminUser()
        {
            var user = new User
            {
                Id = 4,
                FullName = "Himani Jangid",
                Email = "himani.jangid@intimetec.com",
                IsActive = true,
                Role = new Role { Name = "Admin" }
            };

            user.PasswordHash = _hasher.HashPassword(user, "Test@1234");
            return user;
        }

        public static User UserWithMismatchedPassword()
        {
            var user = new User { Id = 2, Email = "vishal@test.com", IsActive = true };
            user.PasswordHash = _hasher.HashPassword(user, "CorrectPass@123");
            return user;
        }

        public static User DeactivatedUser()
        {
            var user = new User
            {
                Id = 5,
                Email = "navneet@test.com",
                IsActive = false,
                Role = new Role { Name = "Organizer" }
            };

            user.PasswordHash = _hasher.HashPassword(user, "Pass@123");
            return user;
        }

        public static User UserWithNullRole()
        {
            var user = new User { Id = 6, Email = "vaishak@test.com", IsActive = true, Role = null };
            user.PasswordHash = _hasher.HashPassword(user, "Pass@123");
            return user;
        }

        public static User MappedUserForRegister(Role role) => new()
        {
            FullName = "Piyush",
            Email = "piyush@test.com",
            Role = role
        };

        public static Role ParticipantRole() => new() { Id = 4, Name = "Participant" };
        public static Role AdminRole() => new() { Id = 1, Name = "Admin" };

        public static LoginResponseDto LoginResponseForHimani() => new()
        {
            Id = 4,
            FullName = "Himani Jangid",
            Email = "himani.jangid@intimetec.com",
            Role = "Admin"
        };

        public static LoginResponseDto LoginResponseForPiyush() => new()
        {
            Id = 22,
            FullName = "Piyush",
            Email = "piyush@test.com",
            Role = "Participant"
        };
    }
}