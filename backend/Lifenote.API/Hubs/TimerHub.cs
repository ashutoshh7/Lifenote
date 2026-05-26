using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Lifenote.API.Hubs;

[Authorize]
public class TimerHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }

    // Clients will call this to start a timer
    public async Task StartTimer(string timerId, int durationMinutes, DateTime startedAt)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            // Broadcast to all other connections of the same user
            await Clients.GroupExcept(userId, Context.ConnectionId)
                .SendAsync("TimerStarted", timerId, durationMinutes, startedAt);
        }
    }

    // Clients will call this to pause a timer
    public async Task PauseTimer(string timerId, int remainingSeconds)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.GroupExcept(userId, Context.ConnectionId)
                .SendAsync("TimerPaused", timerId, remainingSeconds);
        }
    }

    // Clients will call this to reset a timer
    public async Task ResetTimer(string timerId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.GroupExcept(userId, Context.ConnectionId)
                .SendAsync("TimerReset", timerId);
        }
    }
}
