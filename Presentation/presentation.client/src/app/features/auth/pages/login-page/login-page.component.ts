import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { ProgramConfigurationService } from '../../../../core/configuration/program-configuration.service';
import { getApiErrorMessage } from '../../../../core/http/api-error';

@Component({ selector: 'app-login-page', templateUrl: './login-page.component.html', styleUrl: './login-page.component.css', standalone: false })
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  passwordVisible = false;
  loading = false;
  errorMessage = '';
  readonly programConfiguration = inject(ProgramConfigurationService).configuration;
  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  constructor(private readonly auth: AuthService, private readonly route: ActivatedRoute, private readonly router: Router) {}

  submit(): void {
    if (this.form.invalid || this.loading) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.errorMessage = '';
    this.auth.login(this.form.getRawValue()).pipe(finalize(() => this.loading = false)).subscribe({
      next: result => {
        if (!result.data) { this.errorMessage = result.fail?.detail || result.fail?.title || 'Giriş yapılamadı.'; return; }
        void this.router.navigateByUrl(this.route.snapshot.queryParamMap.get('returnUrl') || '/dashboard');
      },
      error: error => this.errorMessage = getApiErrorMessage(error)
    });
  }
}
