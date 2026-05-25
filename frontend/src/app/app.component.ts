import { Component, inject, signal, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { DesktopSidebarComponent } from './layout/navigation/desktop-sidebar/desktop-sidebar.component';
import { MobileBottomNavComponent } from './layout/navigation/mobile-bottom-nav/mobile-bottom-nav.component';
import { AppHeaderComponent } from './layout/header/app-header.component';
import { LayoutService } from './core/services/layout.service';
import { ThemeService } from './core/services/theme.service';
import { AuthService } from './core/services/auth.service';
import { ToastService } from './core/services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    DesktopSidebarComponent,
    MobileBottomNavComponent,
    AppHeaderComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  layoutService = inject(LayoutService);
  authService = inject(AuthService);
  toastService = inject(ToastService);
  private router = inject(Router);
  private themeService = inject(ThemeService);

  isLoginRoute = signal(false);
  private navSub: ReturnType<typeof this.router.events.subscribe> | null = null;
  @ViewChild('mainContent') mainContent?: ElementRef<HTMLElement>;

  constructor() {
    this.themeService.initializeTheme();
  }

  ngOnInit() {
    const idToken = localStorage.getItem('toxin');
    if (idToken) {
      this.authService.isAuthenticated.set(true);
      this.authService.getCurrentUserDetails().subscribe();
    }
    this.updateLoginRoute();
    this.navSub = this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.updateLoginRoute();
        setTimeout(() => {
          if (this.mainContent?.nativeElement) {
            this.mainContent.nativeElement.scrollTo(0, 0);
          } else {
            window.scrollTo(0, 0);
          }
        }, 0);
      });
  }

  ngOnDestroy() {
    this.navSub?.unsubscribe();
  }

  private updateLoginRoute(): void {
    this.isLoginRoute.set(this.router.url === '/login' || this.router.url.startsWith('/login?'));
  }

  showLayout(): boolean {
    return this.authService.isAuthenticated() && !this.isLoginRoute();
  }
}
