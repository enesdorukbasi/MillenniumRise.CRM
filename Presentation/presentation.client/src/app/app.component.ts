import { Component, OnInit } from '@angular/core';
import { ProgramConfigurationService } from './core/configuration/program-configuration.service';
import { ThemeService } from './core/theme/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  constructor(
    private readonly programConfigurationService: ProgramConfigurationService,
    private readonly themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.themeService.initialize();
    this.programConfigurationService.get().subscribe({ error: () => undefined });
  }
}
