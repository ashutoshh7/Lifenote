import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Theme, ThemeService } from '../../../../core/services/theme.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { SettingsService, UserPreferenceDto } from '../../services/settings.service';
import { ToastService } from '../../../../core/services/toast.service';
import { form, required, minLength, validateTree, FormRoot, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule, FormsModule, FormRoot, FormField],
  templateUrl: './settings-page.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./settings-page.component.scss'],
})
export class SettingsPageComponent implements OnInit {
  themeService = inject(ThemeService);
  authService = inject(AuthService);
  private router = inject(Router);
  settingsService = inject(SettingsService);
  toastService = inject(ToastService);

  Theme = Theme;
  currentTheme = signal<Theme>(this.themeService.getTheme());
  accentColor = signal<string>(this.themeService.getAccent());

  reminders = signal(true);
  goalAlerts = signal(true);

  isEditingProfile = signal(false);
  
  // Profile Form (Signal Form)
  profileModel = signal({
    firstName: '',
    lastName: '',
    bio: '',
    dateOfBirth: ''
  });

  profileForm = form(this.profileModel, (schema) => {
    required(schema.firstName);
    required(schema.lastName);
  });

  // Account & Security sub-sections state
  isChangingPassword = signal(false);

  // Password Form (Signal Form)
  passwordModel = signal({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  passwordForm = form(this.passwordModel, (schema) => {
    required(schema.currentPassword);
    required(schema.newPassword);
    minLength(schema.newPassword, 6);
    required(schema.confirmPassword);
    validateTree(schema, (ctx) => {
      if (ctx.value().newPassword !== ctx.value().confirmPassword) {
        return { kind: 'passwordMismatch', message: 'New passwords do not match' };
      }
      return [];
    });
  });

  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);
  isSubmittingPassword = signal(false);

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
      error: (err: any) => this.toastService.show('Failed to load settings', 'error')
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

  get memberSinceYear(): string {
    const d = this.authService.currentUserDetails();
    if (d?.createdAt) {
      return new Date(d.createdAt).getFullYear().toString();
    }
    return new Date().getFullYear().toString();
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
      error: (err: any) => this.toastService.show('Failed to update settings', 'error')
    });
  }

  get profilePictureUrl(): string | null {
    const details = this.authService.currentUserDetails();
    return details?.profilePictureUrl || details?.profilePicture || null;
  }

  toggleEditProfile() {
    if (this.isEditingProfile()) {
      this.isEditingProfile.set(false);
    } else {
      const details = this.authService.currentUserDetails();
      let dobStr = '';
      if (details?.dateOfBirth) {
        const date = new Date(details.dateOfBirth);
        if (!isNaN(date.getTime())) {
          dobStr = date.toISOString().substring(0, 10);
        }
      }
      
      this.profileModel.set({
        firstName: details?.firstName ?? '',
        lastName: details?.lastName ?? '',
        bio: details?.bio ?? '',
        dateOfBirth: dobStr
      });
      
      this.isEditingProfile.set(true);
    }
  }

  saveProfile() {
    if (!this.profileForm().valid()) {
      this.toastService.show('Please fill in all required fields.', 'error');
      return;
    }

    const value = this.profileForm().value();
    const profile = {
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      bio: value.bio.trim() || null,
      dateOfBirth: value.dateOfBirth ? new Date(value.dateOfBirth) : null
    };

    this.authService.updateProfile(profile).subscribe({
      next: () => {
        this.toastService.show('Profile updated successfully!', 'success');
        this.isEditingProfile.set(false);
      },
      error: (err: any) => {
        this.toastService.show('Failed to update profile.', 'error');
      }
    });
  }

  changeAvatar() {
    const currentUrl = this.profilePictureUrl ?? '';
    const newUrl = prompt('Enter a new avatar image URL:', currentUrl);
    if (newUrl !== null) {
      this.authService.updateProfilePicture(newUrl.trim()).subscribe({
        next: () => this.toastService.show('Profile picture updated!', 'success'),
        error: (err: any) => this.toastService.show('Failed to update profile picture', 'error')
      });
    }
  }

  deactivateAccount() {
    if (confirm('WARNING: Are you sure you want to deactivate your account? This will permanently delete your user profile and log you out.')) {
      if (confirm('Please confirm once more that you want to delete your profile.')) {
        this.authService.deactivateAccount().subscribe({
          next: () => {
            this.toastService.show('Account deactivated successfully.', 'success');
            this.router.navigate(['/login']);
          },
          error: (err: any) => this.toastService.show('Failed to deactivate account', 'error')
        });
      }
    }
  }

  // --- Change Password ---
  toggleChangePassword() {
    if (this.authService.isGoogleUser()) {
      this.isChangingPassword.set(!this.isChangingPassword());
      return;
    }
    if (this.isChangingPassword()) {
      this.isChangingPassword.set(false);
    } else {
      this.passwordModel.set({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
      this.showCurrentPassword.set(false);
      this.showNewPassword.set(false);
      this.showConfirmPassword.set(false);
      this.isChangingPassword.set(true);
    }
  }

  togglePasswordVisibility(field: 'current' | 'new' | 'confirm') {
    if (field === 'current') this.showCurrentPassword.set(!this.showCurrentPassword());
    if (field === 'new') this.showNewPassword.set(!this.showNewPassword());
    if (field === 'confirm') this.showConfirmPassword.set(!this.showConfirmPassword());
  }

  submitChangePassword() {
    if (!this.passwordForm().valid()) {
      if (this.passwordForm.newPassword().getError('minLength')) {
         this.toastService.show('Password must be at least 6 characters.', 'error');
      } else if (this.passwordForm().getError('passwordMismatch')) {
         this.toastService.show('New passwords do not match.', 'error');
      } else {
         this.toastService.show('Please complete the password form correctly.', 'error');
      }
      return;
    }

    this.isSubmittingPassword.set(true);
    const value = this.passwordForm().value();
    
    this.authService.changePassword(value.currentPassword, value.newPassword).subscribe({
      next: () => {
        this.toastService.show('Password changed successfully!', 'success');
        this.isChangingPassword.set(false);
        this.isSubmittingPassword.set(false);
      },
      error: (err: any) => {
        this.isSubmittingPassword.set(false);
        let errorMsg = 'Failed to change password.';
        if (err.code === 'auth/wrong-password') {
          errorMsg = 'Incorrect current password.';
        } else if (err.code === 'auth/weak-password') {
          errorMsg = 'New password is too weak.';
        } else if (err.code === 'auth/requires-recent-login') {
          errorMsg = 'For security, please log out and log back in to perform this operation.';
        } else if (err.message) {
          errorMsg = err.message;
        }
        this.toastService.show(errorMsg, 'error');
      }
    });
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
