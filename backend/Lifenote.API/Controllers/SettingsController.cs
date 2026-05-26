using Lifenote.Application.DTOs.Settings;
using Lifenote.Domain.Entities;
using Lifenote.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lifenote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly LifenoteDbContext _context;

        public SettingsController(LifenoteDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        [HttpGet]
        public async Task<ActionResult<UserPreferenceDto>> GetSettings()
        {
            var userId = GetUserId();
            var pref = await _context.UserPreferences.FirstOrDefaultAsync(u => u.UserId == userId);
            
            if (pref == null)
            {
                return Ok(new UserPreferenceDto());
            }

            return Ok(new UserPreferenceDto
            {
                UI = new UISettingsDto
                {
                    Theme = pref.UI.Theme,
                    AccentColor = pref.UI.AccentColor
                },
                Notifications = new NotificationSettingsDto
                {
                    RemindersEnabled = pref.Notifications.RemindersEnabled,
                    GoalAlertsEnabled = pref.Notifications.GoalAlertsEnabled
                }
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] UserPreferenceDto dto)
        {
            var userId = GetUserId();
            var pref = await _context.UserPreferences.FirstOrDefaultAsync(u => u.UserId == userId);

            if (pref == null)
            {
                pref = new UserPreference
                {
                    UserId = userId,
                    UI = new UISettings(),
                    Notifications = new NotificationSettings()
                };
                _context.UserPreferences.Add(pref);
            }

            pref.UI.Theme = dto.UI.Theme;
            pref.UI.AccentColor = dto.UI.AccentColor;
            pref.Notifications.RemindersEnabled = dto.Notifications.RemindersEnabled;
            pref.Notifications.GoalAlertsEnabled = dto.Notifications.GoalAlertsEnabled;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
