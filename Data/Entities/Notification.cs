using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }               
        public int EventRequestId { get; set; } 
        public NotificationAudience Audience {get;set;}  
        public EventRequest? EventRequest {get;set;}
        public string Message { get; set; } = "";     
        public NotificationType Type { get; set; }  
        public bool IsRead {get;set;} = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}