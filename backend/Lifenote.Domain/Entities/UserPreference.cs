namespace Lifenote.Domain.Entities;

public class UserPreference
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // Firebase UID

    // Mapped as a JSON Column via EF Core `ToJson()`
    public UISettings UI { get; set; } = new();

    // Mapped as a JSON Column via EF Core `ToJson()`
    public NotificationSettings Notifications { get; set; } = new();
}

public class UISettings
{
    public string Theme { get; set; } = "system"; // light, dark, system
    public string AccentColor { get; set; } = "#53e076";
}

public class NotificationSettings
{
    public bool RemindersEnabled { get; set; } = true;
    public bool GoalAlertsEnabled { get; set; } = true;
}
