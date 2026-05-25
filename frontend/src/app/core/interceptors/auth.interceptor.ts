import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '@angular/fire/auth';
import { from, switchMap } from 'rxjs';

/**
 * HTTP Interceptor for Angular 20+ (functional style)
 * Automatically adds Firebase ID token to all HTTP requests,
 * and fetches a fresh token dynamically if the current one has expired.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(Auth);
  const currentUser = auth.currentUser;
  
  if (currentUser) {
    // getIdToken(false) fetches the cached token or refreshes it automatically if expired
    return from(currentUser.getIdToken(false)).pipe(
      switchMap(token => {
        localStorage.setItem('toxin', token);
        const clonedRequest = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        return next(clonedRequest);
      })
    );
  }

  const token = localStorage.getItem('toxin');
  if (token) {
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedRequest);
  }

  return next(req);
};