﻿namespace SportsManagementApp.Enums
{
    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Withdrawn = 3,
    }

    public enum GenderType
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        Mixed = 3,
        Both = 4,
    }

    public enum MatchFormat
    {
        Unknown = 0,
        Singles = 1,
        Doubles = 2,
        Both = 3,
    }

    public enum EventStatus
    {
        Unknown = 0,
        Open = 1,
        Upcoming = 2,
        Live = 3,
        Completed = 4,
        Cancelled = 5
    }

    public enum CategoryStatus
    {
        Unknown = 0,
        Active = 1,
        Abandoned = 2,
    }

    public enum MatchStatus
    {
        Unknown = 0,
        Upcoming = 1,
        Live = 2,
        Completed = 3,
    }

    public enum TournamentType
    {
        Unknown = 0,
        Knockout = 1,
        RoundRobin = 2,
    }

    public enum SetStatus
    {
        Unknown = 0,
        NotStarted = 1,
        Live = 2,
        Completed = 3,
        Cancelled = 4,
    }
   public enum NotificationType
    {
        Unknown = 0,
        Approved = 1,
        Rejected = 2,
    }

    public enum NotificationAudience
    {
        Ops = 1,
        Admin = 2
    }
}
