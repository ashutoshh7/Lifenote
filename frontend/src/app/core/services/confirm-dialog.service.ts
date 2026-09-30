import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable, map } from 'rxjs';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

export interface ConfirmDialogOptions {
  title?: string;
  message: string;
  itemTitle?: string;
  confirmText?: string;
  cancelText?: string;
  variant?: 'danger' | 'warning' | 'primary';
  icon?: string;
}

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  private dialog = inject(MatDialog);

  /**
   * Dynamically opens the confirmation dialog via CDK Overlay.
   * Completely self-contained: requires zero template declarations in app.component.html.
   * Returns an Observable<boolean> stream that completes when the dialog closes.
   */
  confirm(options: ConfirmDialogOptions): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: options,
      panelClass: 'confirm-dialog-panel',
      backdropClass: 'confirm-dialog-backdrop',
      autoFocus: false,
      restoreFocus: true
    });

    return dialogRef.afterClosed().pipe(map(result => !!result));
  }
}
