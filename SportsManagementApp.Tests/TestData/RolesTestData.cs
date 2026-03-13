using SportsManagementApp.Data.DTOs.RoleManagement;
using SportsManagementApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class RolesTestData
    {
        public static CreateRoleDto ValidCreateRoleDto() => new() { RoleName = "Manager" };
        public static CreateRoleDto DuplicateRoleDto() => new() { RoleName = "Admin" };

        public static Role ExistingAdminRole() => new() { Id = 1, Name = "Admin" };
        public static Role NewManagerRole() => new() { Id = 5, Name = "Manager" };

        public static List<Role> AllRolesList() => new()
        {
            new() { Id = 1, Name = "Admin" },
            new() { Id = 2, Name = "OpsTeam" },
            new() { Id = 3, Name = "Organizer" },
            new() { Id = 4, Name = "Participant" }
        };

        public static List<Role> EmptyRolesList() => new();
    }
}
