import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
  id: string;
  message: string;
  type: 'success' | 'error';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<ToastMessage[]>([]);
  private nextId = 0;

  show(message: string, type: 'success' | 'error' = 'success'): void {
    const id = (this.nextId++).toString();
    const newToast: ToastMessage = { id, message, type };
    this.toasts.update(current => [...current, newToast]);

    setTimeout(() => {
      this.toasts.update(current => current.filter(t => t.id !== id));
    }, 3000);
  }
}
