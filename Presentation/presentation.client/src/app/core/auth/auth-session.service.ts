import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AppUser, AuthSession } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly storageKey = 'millennium-rise-admin-session';
  private readonly sessionSubject = new BehaviorSubject<AuthSession | null>(this.readSession());

  readonly session$ = this.sessionSubject.asObservable();

  get token(): string | null { return this.validSession?.token ?? null; }
  get user(): AppUser | null { return this.validSession?.user ?? null; }
  get isAuthenticated(): boolean { return this.validSession !== null; }

  start(session: AuthSession): void {
    sessionStorage.setItem(this.storageKey, JSON.stringify(session));
    this.sessionSubject.next(session);
  }

  clear(): void {
    sessionStorage.removeItem(this.storageKey);
    this.sessionSubject.next(null);
  }

  private get validSession(): AuthSession | null {
    const session = this.sessionSubject.value;
    if (!session || new Date(session.expiration).getTime() <= Date.now()) {
      if (session) this.clear();
      return null;
    }
    return session;
  }

  private readSession(): AuthSession | null {
    const rawSession = sessionStorage.getItem(this.storageKey);
    if (!rawSession) return null;

    try {
      const session = JSON.parse(rawSession) as AuthSession;
      if (!session.token || !session.user || new Date(session.expiration).getTime() <= Date.now()) {
        sessionStorage.removeItem(this.storageKey);
        return null;
      }
      return session;
    } catch {
      sessionStorage.removeItem(this.storageKey);
      return null;
    }
  }
}
