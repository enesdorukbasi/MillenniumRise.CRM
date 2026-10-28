import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiResult } from '../models/api.models';
import { ProgramConfiguration, ProgramConfigurationRequest } from '../models/crm.models';

@Injectable({ providedIn: 'root' })
export class ProgramConfigurationService {
  private readonly endpoint = '/api/ProgramConfiguration';
  private readonly configurationState = signal<ProgramConfiguration | null>(null);
  readonly configuration = this.configurationState.asReadonly();

  constructor(
    private readonly http: HttpClient,
    @Inject(DOCUMENT) private readonly document: Document
  ) {}

  get(): Observable<ApiResult<ProgramConfiguration>> {
    return this.http.get<ApiResult<ProgramConfiguration>>(this.endpoint).pipe(
      tap(response => {
        if (response.data) {
          this.configurationState.set(response.data);
          this.document.title = response.data.name;
        }
      })
    );
  }

  update(request: ProgramConfigurationRequest): Observable<ApiResult<ProgramConfiguration>> {
    return this.http.put<ApiResult<ProgramConfiguration>>(this.endpoint, request).pipe(
      tap(response => {
        if (response.data) {
          this.configurationState.set(response.data);
          this.document.title = response.data.name;
        }
      })
    );
  }
}
