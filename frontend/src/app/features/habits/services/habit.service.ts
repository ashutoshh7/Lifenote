import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import {
  Habit,
  CreateHabitDto,
  CheckInDto,
  HabitLog,
  HabitStatistics,
  WeeklyCalendar
} from '../models/habit.model';
import { ApiResponse } from '../../../core/models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class HabitService {
  private apiUrl = `${environment.apiHost}/habits`;

  constructor(private http: HttpClient) {}

  // CRUD
  getHabits(includeInactive: boolean = false): Observable<Habit[]> {
    const params = new HttpParams().set('includeInactive', includeInactive);
    return this.http.get<ApiResponse<Habit[]>>(this.apiUrl, { params })
      .pipe(map(res => res.data ?? []));
  }

  getHabitById(id: number): Observable<Habit> {
    return this.http.get<ApiResponse<Habit>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  createHabit(dto: CreateHabitDto): Observable<Habit> {
    return this.http.post<ApiResponse<Habit>>(this.apiUrl, dto)
      .pipe(map(res => res.data));
  }

  updateHabit(id: number, dto: Partial<CreateHabitDto>): Observable<Habit> {
    return this.http.put<ApiResponse<Habit>>(`${this.apiUrl}/${id}`, dto)
      .pipe(map(res => res.data));
  }

  deleteHabit(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleHabitStatus(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/toggle`, {});
  }

  // Check-in
  checkIn(dto: CheckInDto): Observable<HabitLog> {
    return this.http.post<ApiResponse<HabitLog>>(`${this.apiUrl}/checkin`, dto)
      .pipe(map(res => res.data));
  }

  undoCheckIn(habitId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${habitId}/checkin`);
  }

  // Analytics
  getHabitHistory(
    habitId: number,
    startDate?: string,
    endDate?: string
  ): Observable<HabitLog[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    return this.http.get<ApiResponse<HabitLog[]>>(`${this.apiUrl}/${habitId}/history`, { params })
      .pipe(map(res => res.data ?? []));
  }

  getHabitStatistics(habitId: number): Observable<HabitStatistics> {
    return this.http.get<ApiResponse<HabitStatistics>>(`${this.apiUrl}/${habitId}/statistics`)
      .pipe(map(res => res.data));
  }

  getWeeklyCalendar(weekStart?: string): Observable<WeeklyCalendar> {
    let params = new HttpParams();
    if (weekStart) params = params.set('weekStart', weekStart);
    return this.http.get<ApiResponse<WeeklyCalendar>>(`${this.apiUrl}/calendar/weekly`, { params })
      .pipe(map(res => res.data));
  }
}
