import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { AuthSessionService } from './auth-session.service';

export const guestGuard: CanMatchFn = () => {
  const session = inject(AuthSessionService);
  const router = inject(Router);
  return session.isAuthenticated ? router.createUrlTree(['/dashboard']) : true;
};
