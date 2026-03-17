using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Predicates;
public static class NotificationPredicateBuilder
{
    public static Expression<Func<Notification,bool>> Ops()
    {
        return n => n.Audience == NotificationAudience.Ops;
    }

    public static Expression<Func<Notification,bool>> Admin(int adminId)
    {
        return n => n.Audience == NotificationAudience.Admin
                    && n.UserId == adminId;
    }

    public static Expression<Func<Notification, bool>> UnreadOps(){
        return n => n.Audience == NotificationAudience.Ops && !n.IsRead;
    }

    public static Expression<Func<Notification, bool>> UnreadAdmin(int adminId)
        => n => n.Audience == NotificationAudience.Admin &&
                n.UserId == adminId &&
                !n.IsRead;
}