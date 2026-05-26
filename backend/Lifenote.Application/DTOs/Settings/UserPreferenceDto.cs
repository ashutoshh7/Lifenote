namespace Lifenote.Application.DTOs.Settings;

public class UserPreferenceDto
{
    public UISettingsDto UI { get; set; } = new();
    public NotificationSettingsDto Notifications { get; set; } = new();
}

public class UISettingsDto
{
    public string Theme { get; set; } = "system";
    public string AccentColor { get; set; } = "#53e076";
}

public class NotificationSettingsDto
{
    public bool RemindersEnabled { get; set; } = true;
    public bool GoalAlertsEnabled { get; set; } = true;
}
