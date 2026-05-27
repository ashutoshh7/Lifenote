import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, of, switchMap, tap, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { INote, ICreateNoteDto, IUpdateNoteDto } from '../../../../../core/models/note.model';
import { ApiResponse } from '../../../../../core/models/api-response.model';

@Injectable({
  providedIn: 'root',
})
export class NotesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiHost}/Note`;

  // Signals for reactive state
  notes = signal<INote[]>([]);
  searchTerm = signal<string>('');
  isLoading = signal<boolean>(false);

  getAllNotes(): Observable<INote[]> {
    this.isLoading.set(true);
    return this.http.get<ApiResponse<INote[]>>(this.apiUrl).pipe(
      map(res => res.data ?? []),
      tap(notes => {
        this.notes.set(notes);
        this.isLoading.set(false);
      })
    );
  }

  getNoteById(id: string): Observable<INote> {
    return this.http.get<ApiResponse<INote>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  createNote(note: ICreateNoteDto): Observable<INote> {
    return this.http.post<ApiResponse<INote>>(this.apiUrl, note).pipe(
      map(res => res.data),
      tap(newNote => {
        this.notes.update(notes => [newNote, ...notes]);
      })
    );
  }

  updateNote(id: string, note: IUpdateNoteDto): Observable<INote> {
    return this.http.put<ApiResponse<INote>>(`${this.apiUrl}/${id}`, note).pipe(
      map(res => res.data),
      switchMap(updatedNote => of(updatedNote)),
      tap(updatedNote => {
        this.notes.update(notes =>
          notes.map(n => n.id === id ? { ...updatedNote } : n)
        );
      })
    );
  }

  deleteNote(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.notes.update(notes => notes.filter(n => n.id !== id));
      })
    );
  }

  togglePin(id: string): Observable<INote> {
    return this.http.patch<ApiResponse<INote>>(`${this.apiUrl}/${id}/pin`, {}).pipe(
      map(res => res.data),
      tap(updatedNote => {
        this.notes.update(notes =>
          notes.map(n => n.id === id ? updatedNote : n)
        );
      })
    );
  }

  searchNotes(term: string): Observable<INote[]> {
    this.searchTerm.set(term);
    return this.http.get<ApiResponse<INote[]>>(`${this.apiUrl}/search?q=${encodeURIComponent(term)}`).pipe(
      map(res => res.data ?? []),
      tap(notes => this.notes.set(notes))
    );
  }

  toggleArchive(id: string): Observable<INote> {
    return this.http.patch<ApiResponse<INote>>(`${this.apiUrl}/${id}/archive`, {}).pipe(
      map(res => res.data),
      tap(updatedNote => {
        this.notes.update(notes =>
          notes.map(n => n.id === id ? updatedNote : n)
        );
      })
    );
  }
}
