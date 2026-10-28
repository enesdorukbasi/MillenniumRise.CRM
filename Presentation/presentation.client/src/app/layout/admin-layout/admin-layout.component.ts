import { Component, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { AuthSessionService } from '../../core/auth/auth-session.service';
import { ProgramConfigurationService } from '../../core/configuration/program-configuration.service';
import { IconName } from '../../shared/components/icon/icon.component';

interface NavigationItem { label: string; path: string; icon: IconName; }

@Component({ selector: 'app-admin-layout', templateUrl: './admin-layout.component.html', styleUrl: './admin-layout.component.css', standalone: false })
export class AdminLayoutComponent {
  private readonly programConfigurationService = inject(ProgramConfigurationService);
  sidebarOpen = false;
  readonly programConfiguration = this.programConfigurationService.configuration;
  readonly navigationItems: NavigationItem[] = [
    { label: 'Genel Bakış', path: '/dashboard', icon: 'dashboard' },
    { label: 'Müşteriler', path: '/customers', icon: 'customers' },
    { label: 'Ürünler', path: '/products', icon: 'products' },
    { label: 'Dosyalar', path: '/files', icon: 'files' },
    { label: 'Program Ayarları', path: '/program-configuration', icon: 'settings' }
  ];

  constructor(readonly session: AuthSessionService, private readonly auth: AuthService, router: Router) {
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => this.sidebarOpen = false);
  }

  get currentDate(): string {
    return new Intl.DateTimeFormat('tr-TR', { day: '2-digit', month: 'long', year: 'numeric', weekday: 'long' }).format(new Date());
  }

  logout(): void { this.auth.logout(); }
}
