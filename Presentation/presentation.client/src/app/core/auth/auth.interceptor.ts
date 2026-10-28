import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthSessionService } from './auth-session.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private readonly session: AuthSessionService, private readonly router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.session.token;
    const authenticatedRequest = token && request.url.startsWith('/api/')
      ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : request;

    return next.handle(authenticatedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !request.url.endsWith('/AppUser/login')) {
          this.session.clear();
          void this.router.navigate(['/login']);
        }
        return throwError(() => error);
      })
    );
  }
}
