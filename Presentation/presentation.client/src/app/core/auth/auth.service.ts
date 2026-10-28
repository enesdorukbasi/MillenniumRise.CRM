import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiResult } from '../models/api.models';
import { AuthSessionService } from './auth-session.service';
import { LoginRequest, LoginResponse } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    private readonly http: HttpClient,
    private readonly session: AuthSessionService,
    private readonly router: Router
  ) {}

  login(request: LoginRequest): Observable<ApiResult<LoginResponse>> {
    return this.http.post<ApiResult<LoginResponse>>('/api/AppUser/login', request).pipe(
      tap(result => {
        if (result.data) this.session.start(result.data);
      })
    );
  }

  logout(): void {
    this.session.clear();
    void this.router.navigate(['/login']);
  }
}
