import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Theme, ThemeService } from '../../../../core/services/theme.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { SettingsService, UserPreferenceDto } from '../../services/settings.service';
@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './settings-page.component.html',
  styleUrls: ['./settings-page.component.scss'],
})
export class SettingsPageComponent implements OnInit {
  themeService = inject(ThemeService);
  authService = inject(AuthService);
  private router = inject(Router);
  settingsService = inject(SettingsService);

  Theme = Theme;
  currentTheme = signal<Theme>(this.themeService.getTheme());
  accentColor = signal<string>('#53e076');

  reminders = signal(true);
  goalAlerts = signal(true);

  ngOnInit() {
    this.settingsService.getSettings().subscribe({
      next: (settings) => {
        if (settings.ui) {
          const t = settings.ui.theme as Theme;
          if (Object.values(Theme).includes(t)) {
            this.currentTheme.set(t);
            this.themeService.setTheme(t);
          }
          if (settings.ui.accentColor) {
            this.accentColor.set(settings.ui.accentColor);
            document.documentElement.style.setProperty('--primary-color', settings.ui.accentColor);
          }
        }
        if (settings.notifications) {
          this.reminders.set(settings.notifications.remindersEnabled);
          this.goalAlerts.set(settings.notifications.goalAlertsEnabled);
        }
      },
      error: (err) => console.error('Failed to load settings', err)
    });
  }

  get userName(): string {
    const d = this.authService.currentUserDetails();
    if (d?.firstName && d?.lastName) return `${d.firstName} ${d.lastName}`;
    if (d?.username) return d.username;
    return 'User';
  }

  get userEmail(): string {
    return this.authService.currentUserDetails()?.email ?? '';
  }

  get userInitial(): string {
    return this.userName.charAt(0).toUpperCase();
  }

  setTheme(theme: Theme) {
    this.themeService.setTheme(theme);
    this.currentTheme.set(theme);
    this.saveSettings();
  }

  setAccentColor(color: string) {
    this.accentColor.set(color);
    document.documentElement.style.setProperty('--primary-color', color);
    this.saveSettings();
  }

  toggleReminders() {
    this.reminders.set(!this.reminders());
    this.saveSettings();
  }

  toggleGoalAlerts() {
    this.goalAlerts.set(!this.goalAlerts());
    this.saveSettings();
  }

  private saveSettings() {
    const dto: UserPreferenceDto = {
      ui: {
        theme: this.currentTheme(),
        accentColor: this.accentColor()
      },
      notifications: {
        remindersEnabled: this.reminders(),
        goalAlertsEnabled: this.goalAlerts()
      }
    };
    this.settingsService.updateSettings(dto).subscribe({
      error: (err) => console.error('Failed to update settings', err)
    });
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
