import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Theme, ThemeService } from '../../../../core/services/theme.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent],
  templateUrl: './settings-page.component.html',
  styleUrls: ['./settings-page.component.scss'],
})
export class SettingsPageComponent {
  themeService = inject(ThemeService);
  authService = inject(AuthService);
  private router = inject(Router);

  Theme = Theme;
  currentTheme = signal<Theme>(this.themeService.getTheme());

  reminders = signal(true);
  goalAlerts = signal(true);

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
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
