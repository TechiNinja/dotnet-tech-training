using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Hubs;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    private static void ValidateAdminId(int adminId)
    {
        if (adminId <= 0)
            throw new ValidationException(StringConstant.InvalidId);
    }

    public async Task<Notification> CreateAsync(BaseNotificationDto dto)
    {
        if (dto.EventRequestId <= 0)
            throw new ValidationException(StringConstant.InvalidId);

        if (string.IsNullOrWhiteSpace(dto.Message))
            throw new ValidationException(StringConstant.MessageRequired);

        if (dto.Audience == NotificationAudience.Admin &&
            (!dto.UserId.HasValue || dto.UserId.Value <= 0))
            throw new ValidationException(StringConstant.IdRequired);

        if (dto.Audience == NotificationAudience.Ops)
            dto.UserId = null;

        dto.Message = dto.Message.Trim();

        var notification = _mapper.Map<Notification>(dto);
        notification.IsRead = false;

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        var payload = _mapper.Map<NotificationResponseDto>(notification);

        if (notification.Audience == NotificationAudience.Ops)
        {
            await _hubContext.Clients.Group("ops")
                .SendAsync(StringConstant.NewNotificationEvent, payload);
        }
        else
        {
            await _hubContext.Clients.Group($"admin:{notification.UserId}")
                .SendAsync(StringConstant.NewNotificationEvent, payload);
        }

        return notification;
    }

    public async Task<List<NotificationResponseDto>> GetOpsNotificationAsync(bool? isRead)
    {
        var notifications = await _notificationRepository.GetOpsNotificationAsync(isRead);
        return _mapper.Map<List<NotificationResponseDto>>(notifications);
    }

    public async Task<List<NotificationResponseDto>> GetAdminNotificationAsync(int adminId, bool? isRead)
    {
        ValidateAdminId(adminId);

        var notifications = await _notificationRepository.GetAdminNotificationAsync(adminId, isRead);
        return _mapper.Map<List<NotificationResponseDto>>(notifications);
    }

    public async Task<int> GetUnreadCountForOpsAsync()
    {
        return await _notificationRepository.GetUnreadCountForOpsAsync();
    }

    public async Task<int> GetUnreadCountForAdminAsync(int adminId)
    {
        ValidateAdminId(adminId);
        return await _notificationRepository.GetUnreadCountForAdminAsync(adminId);
    }

    public async Task MarkOpsAsReadAsync()
    {
        await _notificationRepository.MarkOpsAsReadAsync();
        await _notificationRepository.SaveChangesAsync();
    }

    public async Task MarkAdminAsReadAsync(int adminId)
    {
        ValidateAdminId(adminId);
        await _notificationRepository.MarkAdminAsReadAsync(adminId);
        await _notificationRepository.SaveChangesAsync();
    }
}