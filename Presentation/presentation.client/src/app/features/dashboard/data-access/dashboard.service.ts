import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { CustomerProduct, DashboardSummary, Interaction } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private readonly http: HttpClient) {}

  getSummary(): Observable<ApiResult<DashboardSummary>> {
    return this.http.get<ApiResult<DashboardSummary>>('/api/Dashboard/summary');
  }

  getUpcomingFollowUps(pageNumber: number, pageSize: number): Observable<ApiResult<PagedResult<CustomerProduct>>> {
    const params = new HttpParams().set('PageNumber', pageNumber).set('PageSize', pageSize);
    return this.http.get<ApiResult<PagedResult<CustomerProduct>>>('/api/Dashboard/upcoming-follow-ups', { params });
  }

  getRecentInteractions(pageNumber: number, pageSize: number): Observable<ApiResult<PagedResult<Interaction>>> {
    const params = new HttpParams().set('PageNumber', pageNumber).set('PageSize', pageSize);
    return this.http.get<ApiResult<PagedResult<Interaction>>>('/api/Dashboard/recent-interactions', { params });
  }
}
