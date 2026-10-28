import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createHttpParams } from '../../../core/http/http-params';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { CustomerProduct, CustomerProductListQuery, CustomerProductRequest } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class CustomerProductsService {
  private readonly endpoint = '/api/CustomerProduct';
  constructor(private readonly http: HttpClient) {}
  getList(query: CustomerProductListQuery): Observable<ApiResult<PagedResult<CustomerProduct>>> { return this.http.get<ApiResult<PagedResult<CustomerProduct>>>(this.endpoint, { params: createHttpParams(query) }); }
  create(request: CustomerProductRequest): Observable<ApiResult<CustomerProduct>> { return this.http.post<ApiResult<CustomerProduct>>(this.endpoint, request); }
  update(id: string, request: CustomerProductRequest): Observable<ApiResult<CustomerProduct>> { return this.http.put<ApiResult<CustomerProduct>>(`${this.endpoint}/${id}`, request); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.endpoint}/${id}`); }
}
