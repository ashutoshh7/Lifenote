import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { initializeApp, provideFirebaseApp } from '@angular/fire/app';
import { getAuth, provideAuth } from '@angular/fire/auth';
import { getAnalytics, provideAnalytics, ScreenTrackingService, UserTrackingService } from '@angular/fire/analytics';
import { environment } from './environments/environment';

bootstrapApplication(AppComponent, {
  providers: [
    ...appConfig.providers,
    provideFirebaseApp(() => initializeApp(environment.firebase)), 
    provideAuth(() => getAuth()), 
    provideAnalytics(() => getAnalytics()),
    ScreenTrackingService, 
    UserTrackingService
  ]
}).then(() => {
  // Hide the splash screen once Angular has bootstrapped
  const splash = document.getElementById('app-splash');
  if (splash) {
    splash.classList.add('hidden');
    // Remove from DOM after fade-out transition completes
    splash.addEventListener('transitionend', () => splash.remove(), { once: true });
  }
}).catch(err => console.error(err));