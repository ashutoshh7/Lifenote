import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service';

export enum Theme {
  Light = 'light',
  Dark = 'dark',
  System = 'system',
}

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'theme';
  private authService = inject(AuthService);

  constructor() {}

  getTheme(): Theme {
    return (localStorage.getItem(this.themeKey) as Theme) || Theme.System;
  }

  setTheme(theme: Theme): void {
    localStorage.setItem(this.themeKey, theme);
    this.applyTheme();
    if (this.authService.isAuthenticated()) {
      this.authService.updateTheme(theme).subscribe({
        error: (err: any) => console.error('Failed to sync theme to backend', err)
      });
    }
  }

  initializeTheme(): void {
    this.applyTheme();
    window
      .matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', () => {
        if (this.getTheme() === Theme.System) {
          this.applyTheme();
        }
      });
  }

  private applyTheme(): void {
    const theme = this.getTheme();
    const prefersDark = window.matchMedia(
      '(prefers-color-scheme: dark)'
    ).matches;

    if (theme === Theme.Dark || (theme === Theme.System && prefersDark)) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }
}