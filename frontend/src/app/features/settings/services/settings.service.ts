import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';

export interface UserPreferenceDto {
  ui: {
    theme: string;
    accentColor: string;
  };
  notifications: {
    remindersEnabled: boolean;
    goalAlertsEnabled: boolean;
  };
}

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private http = inject(HttpClient);
  private apiHost = `${environment.apiHost}/Settings`;

  getSettings(): Observable<UserPreferenceDto> {
    return this.http.get<UserPreferenceDto>(this.apiHost);
  }

  updateSettings(settings: UserPreferenceDto): Observable<void> {
    return this.http.put<void>(this.apiHost, settings);
  }
}
