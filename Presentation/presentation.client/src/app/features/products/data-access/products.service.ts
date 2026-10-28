import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createHttpParams } from '../../../core/http/http-params';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { Product, ProductListQuery, ProductRequest } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly endpoint = '/api/Product';
  constructor(private readonly http: HttpClient) {}
  getList(query: ProductListQuery): Observable<ApiResult<PagedResult<Product>>> { return this.http.get<ApiResult<PagedResult<Product>>>(this.endpoint, { params: createHttpParams(query) }); }
  getById(id: string): Observable<ApiResult<Product>> { return this.http.get<ApiResult<Product>>(`${this.endpoint}/${id}`); }
  create(request: ProductRequest): Observable<ApiResult<Product>> { return this.http.post<ApiResult<Product>>(this.endpoint, request); }
  update(id: string, request: ProductRequest): Observable<ApiResult<Product>> { return this.http.put<ApiResult<Product>>(`${this.endpoint}/${id}`, request); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.endpoint}/${id}`); }
}
