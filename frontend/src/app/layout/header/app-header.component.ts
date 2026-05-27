import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { signal } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss']
})
export class AppHeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);

  pageTitle = signal('Dashboard');

  constructor() {
    this.router.events.subscribe(() => {
      const url = this.router.url;
      if (url.includes('/notes')) this.pageTitle.set('Notes');
      else if (url.includes('/goals')) this.pageTitle.set('Goals');
      else if (url.includes('/pomodoro')) this.pageTitle.set('Timer');
      else if (url.includes('/settings')) this.pageTitle.set('Settings');
      else this.pageTitle.set('Dashboard');
    });
  }

  get userName(): string {
    const details = this.authService.currentUserDetails() as any;
    if (details?.firstName) return details.firstName;
    if (details?.username) return details.username;
    return 'User';
  }

  get userInitials(): string {
    const name = this.userName;
    return name.charAt(0).toUpperCase();
  }
}
