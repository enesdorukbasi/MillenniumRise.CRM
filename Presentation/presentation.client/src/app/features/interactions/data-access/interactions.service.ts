import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createHttpParams } from '../../../core/http/http-params';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { Interaction, InteractionListQuery, InteractionRequest } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class InteractionsService {
  private readonly endpoint = '/api/Interaction';
  constructor(private readonly http: HttpClient) {}
  getList(query: InteractionListQuery): Observable<ApiResult<PagedResult<Interaction>>> { return this.http.get<ApiResult<PagedResult<Interaction>>>(this.endpoint, { params: createHttpParams(query) }); }
  create(request: InteractionRequest): Observable<ApiResult<Interaction>> { return this.http.post<ApiResult<Interaction>>(this.endpoint, request); }
  update(id: string, request: InteractionRequest): Observable<ApiResult<Interaction>> { return this.http.put<ApiResult<Interaction>>(`${this.endpoint}/${id}`, request); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.endpoint}/${id}`); }
}
