import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service';
import { ToastService } from './toast.service';

const DEFAULT_ACCENT = '#53e076';

function hexToRgba(hex: string, alpha: number): string {
  const r = parseInt(hex.slice(1, 3), 16);
  const g = parseInt(hex.slice(3, 5), 16);
  const b = parseInt(hex.slice(5, 7), 16);
  return `rgba(${r}, ${g}, ${b}, ${alpha})`;
}

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
  private readonly accentKey = 'accent';
  private authService = inject(AuthService);
  private toastService = inject(ToastService);

  constructor() {}

  getTheme(): Theme {
    return (localStorage.getItem(this.themeKey) as Theme) || Theme.System;
  }

  setTheme(theme: Theme): void {
    localStorage.setItem(this.themeKey, theme);
    this.applyTheme();
    if (this.authService.isAuthenticated()) {
      this.authService.updateTheme(theme).subscribe({
        error: () => this.toastService.show('Failed to sync theme preferences.', 'error')
      });
    }
  }

  getAccent(): string {
    return localStorage.getItem(this.accentKey) || DEFAULT_ACCENT;
  }

  setAccent(hex: string): void {
    localStorage.setItem(this.accentKey, hex);
    this.applyAccent(hex);
  }

  applyAccent(hex: string): void {
    const root = document.documentElement.style;
    root.setProperty('--primary', hex);
    root.setProperty('--ring', hex);
    root.setProperty('--completed', hex);
    root.setProperty('--primary-fixed-dim', hex);
    root.setProperty('--primary-glow', hexToRgba(hex, 0.30));
    root.setProperty('--primary-glow-sm', hexToRgba(hex, 0.15));
  }

  initializeTheme(): void {
    this.applyTheme();
    this.applyAccent(this.getAccent());
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
      document.documentElement.classList.remove('light');
    } else {
      document.documentElement.classList.remove('dark');
      document.documentElement.classList.add('light');
    }
  }
}
