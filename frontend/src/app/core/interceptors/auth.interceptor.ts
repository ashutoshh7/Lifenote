import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth, idToken } from '@angular/fire/auth';
import { from, switchMap, take } from 'rxjs';

/**
 * HTTP Interceptor for Angular 20+ (functional style)
 * Automatically adds Firebase ID token to all HTTP requests,
 * and fetches a fresh token dynamically if the current one has expired.
 * On initial page load/refresh, it defers outgoing requests until Firebase Auth
 * is initialized to avoid unauthorized errors caused by expired local cached tokens.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(Auth);

  // If Firebase Auth is already initialized and we have a current user,
  // we can get the token instantly to avoid any delay.
  if (auth.currentUser) {
    return from(auth.currentUser.getIdToken(false)).pipe(
      switchMap(token => {
        const clonedRequest = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        return next(clonedRequest);
      })
    );
  }

  // Otherwise, wait for the first emission from idToken(auth) to ensure
  // Firebase Auth has finished initializing.
  return idToken(auth).pipe(
    take(1),
    switchMap(token => {
      if (token) {
        const clonedRequest = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        return next(clonedRequest);
      }

      return next(req);
    })
  );
};
