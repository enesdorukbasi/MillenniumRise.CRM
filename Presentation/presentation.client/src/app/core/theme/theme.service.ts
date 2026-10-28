import { DOCUMENT } from '@angular/common';
import { Inject, Injectable, signal } from '@angular/core';

export type ThemePreference = 'light' | 'dark' | 'system';
export type ResolvedTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly storageKey = 'millennium-rise-theme';
  private readonly systemTheme = window.matchMedia('(prefers-color-scheme: dark)');
  private readonly preferenceState = signal<ThemePreference>(this.readPreference());
  private readonly resolvedThemeState = signal<ResolvedTheme>('light');

  readonly preference = this.preferenceState.asReadonly();
  readonly resolvedTheme = this.resolvedThemeState.asReadonly();

  constructor(@Inject(DOCUMENT) private readonly document: Document) {
    this.applyTheme();
    this.systemTheme.addEventListener('change', () => {
      if (this.preferenceState() === 'system') this.applyTheme();
    });
  }

  initialize(): void {
    this.applyTheme();
  }

  setPreference(preference: ThemePreference): void {
    this.preferenceState.set(preference);
    localStorage.setItem(this.storageKey, preference);
    this.applyTheme();
  }

  toggleResolvedTheme(): void {
    this.setPreference(this.resolvedThemeState() === 'dark' ? 'light' : 'dark');
  }

  private applyTheme(): void {
    const preference = this.preferenceState();
    const resolvedTheme: ResolvedTheme = preference === 'system'
      ? (this.systemTheme.matches ? 'dark' : 'light')
      : preference;

    this.resolvedThemeState.set(resolvedTheme);
    this.document.documentElement.dataset['theme'] = resolvedTheme;
    this.document.documentElement.dataset['themePreference'] = preference;

    const themeColor = this.document.querySelector<HTMLMetaElement>('meta[name="theme-color"]');
    if (themeColor) themeColor.content = resolvedTheme === 'dark' ? '#07101d' : '#102a56';
  }

  private readPreference(): ThemePreference {
    const preference = localStorage.getItem(this.storageKey);
    return preference === 'light' || preference === 'dark' || preference === 'system'
      ? preference
      : 'system';
  }
}
