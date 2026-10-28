import { Component, HostListener, OnDestroy, OnInit, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { AuthSessionService } from '../../core/auth/auth-session.service';
import { ProgramConfigurationService } from '../../core/configuration/program-configuration.service';
import { ThemePreference, ThemeService } from '../../core/theme/theme.service';
import { IconName } from '../../shared/components/icon/icon.component';

interface NavigationItem { label: string; path: string; icon: IconName; }

@Component({ selector: 'app-admin-layout', templateUrl: './admin-layout.component.html', styleUrl: './admin-layout.component.css', standalone: false })
export class AdminLayoutComponent implements OnInit, OnDestroy {
  private readonly programConfigurationService = inject(ProgramConfigurationService);
  private readonly themeService = inject(ThemeService);
  private readonly dateTimeFormatter = new Intl.DateTimeFormat('tr-TR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
    weekday: 'long',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false
  });
  private clockIntervalId: number | undefined;
  sidebarOpen = false;
  sidebarCollapsed = false;
  userMenuOpen = false;
  themeMenuOpen = false;
  currentDateTime = this.dateTimeFormatter.format(new Date());
  readonly programConfiguration = this.programConfigurationService.configuration;
  readonly themePreference = this.themeService.preference;
  readonly resolvedTheme = this.themeService.resolvedTheme;
  readonly navigationItems: NavigationItem[] = [
    { label: 'Genel Bakış', path: '/dashboard', icon: 'dashboard' },
    { label: 'Müşteriler', path: '/customers', icon: 'customers' },
    { label: 'Ürünler', path: '/products', icon: 'products' },
    { label: 'Dosyalar', path: '/files', icon: 'files' },
    { label: 'Program Ayarları', path: '/program-configuration', icon: 'settings' }
  ];

  constructor(readonly session: AuthSessionService, private readonly auth: AuthService, router: Router) {
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      this.sidebarOpen = false;
      this.userMenuOpen = false;
      this.themeMenuOpen = false;
    });
  }

  ngOnInit(): void {
    this.clockIntervalId = window.setInterval(() => {
      this.currentDateTime = this.dateTimeFormatter.format(new Date());
    }, 1000);
  }

  ngOnDestroy(): void {
    if (this.clockIntervalId !== undefined) window.clearInterval(this.clockIntervalId);
  }

  get sidebarToggleLabel(): string {
    if (window.innerWidth <= 900) return this.sidebarOpen ? 'Menüyü kapat' : 'Menüyü aç';
    return this.sidebarCollapsed ? 'Menüyü genişlet' : 'Menüyü daralt';
  }

  get sidebarExpanded(): boolean {
    return window.innerWidth <= 900 ? this.sidebarOpen : !this.sidebarCollapsed;
  }

  get sidebarToggleIcon(): IconName {
    if (window.innerWidth <= 900) return this.sidebarOpen ? 'chevrons-left' : 'chevrons-right';
    return this.sidebarCollapsed ? 'chevrons-right' : 'chevrons-left';
  }

  get sidebarBrandFontSize(): string {
    const nameLength = (this.programConfiguration()?.name || 'Millennium Rise').length;
    if (nameLength <= 18) return '1.02rem';
    if (nameLength <= 24) return '.9rem';
    if (nameLength <= 32) return '.78rem';
    if (nameLength <= 42) return '.68rem';
    return '.6rem';
  }

  get userInitials(): string {
    const nameParts = this.session.user?.fullName?.trim().split(/\s+/).filter(Boolean) ?? [];
    if (!nameParts.length) return 'A';
    if (nameParts.length === 1) return nameParts[0].slice(0, 2).toUpperCase();
    return `${nameParts[0][0]}${nameParts[nameParts.length - 1][0]}`.toUpperCase();
  }

  get themePreferenceLabel(): string {
    if (this.themePreference() === 'dark') return 'Koyu';
    if (this.themePreference() === 'light') return 'Açık';
    return 'Sistem';
  }

  get themePreferenceIcon(): IconName {
    if (this.themePreference() === 'dark') return 'moon';
    if (this.themePreference() === 'light') return 'sun';
    return 'monitor';
  }

  toggleSidebar(): void {
    if (window.innerWidth <= 900) {
      this.sidebarCollapsed = false;
      this.sidebarOpen = !this.sidebarOpen;
      return;
    }

    this.sidebarOpen = false;
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  toggleUserMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.userMenuOpen = !this.userMenuOpen;
  }

  toggleThemeMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.themeMenuOpen = !this.themeMenuOpen;
  }

  selectTheme(preference: ThemePreference, event: MouseEvent): void {
    event.stopPropagation();
    this.themeService.setPreference(preference);
    this.themeMenuOpen = false;
  }

  toggleCollapsedTheme(event: MouseEvent): void {
    event.stopPropagation();
    this.themeService.toggleResolvedTheme();
  }

  @HostListener('document:click')
  closeUserMenu(): void {
    this.userMenuOpen = false;
    this.themeMenuOpen = false;
  }

  @HostListener('window:resize')
  handleResize(): void {
    if (window.innerWidth <= 900) this.sidebarCollapsed = false;
    else this.sidebarOpen = false;
  }

  logout(): void {
    this.userMenuOpen = false;
    this.auth.logout();
  }
}
