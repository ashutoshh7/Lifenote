import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Theme, ThemeService } from '../../../../core/services/theme.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { SettingsService, UserPreferenceDto } from '../../services/settings.service';
@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
  accentColor = signal<string>(this.themeService.getAccent());

  reminders = signal(true);
  goalAlerts = signal(true);

  isEditingProfile = signal(false);
  editFirstName = signal('');
  editLastName = signal('');
  editBio = signal('');
  editDateOfBirth = signal('');

  ngOnInit() {
    this.settingsService.getSettings().subscribe({
      next: (settings: any) => {
        if (settings.ui) {
          const t = settings.ui.theme as Theme;
          if (Object.values(Theme).includes(t)) {
            this.currentTheme.set(t);
            this.themeService.setTheme(t);
          }
          if (settings.ui.accentColor) {
            this.accentColor.set(settings.ui.accentColor);
            this.themeService.setAccent(settings.ui.accentColor);
          }
        }
        if (settings.notifications) {
          this.reminders.set(settings.notifications.remindersEnabled);
          this.goalAlerts.set(settings.notifications.goalAlertsEnabled);
        }
      },
      error: (err: any) => console.error('Failed to load settings', err)
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

  readonly PRESET_ACCENTS = ['#53e076','#3d9bff','#a78bfa','#f472b6','#fb923c','#facc15','#22d3ee','#f87171'];

  setAccentColor(color: string) {
    this.accentColor.set(color);
    this.themeService.setAccent(color);
    this.saveSettings();
  }

  isCustomAccent(): boolean {
    return !this.PRESET_ACCENTS.includes(this.accentColor());
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
      error: (err: any) => console.error('Failed to update settings', err)
    });
  }

  get profilePictureUrl(): string | null {
    const details = this.authService.currentUserDetails();
    return details?.profilePictureUrl || details?.profilePicture || null;
  }

  startEditProfile() {
    const details = this.authService.currentUserDetails();
    this.editFirstName.set(details?.firstName ?? '');
    this.editLastName.set(details?.lastName ?? '');
    this.editBio.set(details?.bio ?? '');
    if (details?.dateOfBirth) {
      const date = new Date(details.dateOfBirth);
      if (!isNaN(date.getTime())) {
        this.editDateOfBirth.set(date.toISOString().substring(0, 10));
      } else {
        this.editDateOfBirth.set('');
      }
    } else {
      this.editDateOfBirth.set('');
    }
    this.isEditingProfile.set(true);
  }

  saveProfile() {
    const profile = {
      firstName: this.editFirstName().trim(),
      lastName: this.editLastName().trim(),
      bio: this.editBio().trim() || null,
      dateOfBirth: this.editDateOfBirth() ? new Date(this.editDateOfBirth()) : null
    };

    if (!profile.firstName || !profile.lastName) {
      alert('First name and last name are required.');
      return;
    }

    this.authService.updateProfile(profile).subscribe({
      next: () => {
        this.isEditingProfile.set(false);
      },
      error: (err: any) => console.error('Failed to update profile', err)
    });
  }

  cancelEditProfile() {
    this.isEditingProfile.set(false);
  }

  changeAvatar() {
    const currentUrl = this.profilePictureUrl ?? '';
    const newUrl = prompt('Enter a new avatar image URL:', currentUrl);
    if (newUrl !== null) {
      this.authService.updateProfilePicture(newUrl.trim()).subscribe({
        error: (err: any) => console.error('Failed to update profile picture', err)
      });
    }
  }

  deactivateAccount() {
    if (confirm('WARNING: Are you sure you want to deactivate your account? This will permanently delete your user profile and log you out.')) {
      if (confirm('Please confirm once more that you want to delete your profile.')) {
        this.authService.deactivateAccount().subscribe({
          next: () => {
            this.router.navigate(['/login']);
          },
          error: (err: any) => console.error('Failed to deactivate account', err)
        });
      }
    }
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
