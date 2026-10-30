import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createHttpParams } from '../../../core/http/http-params';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { Customer, CustomerImportResult, CustomerListQuery, CustomerRequest } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class CustomersService {
  private readonly endpoint = '/api/Customer';
  constructor(private readonly http: HttpClient) {}

  getList(query: CustomerListQuery): Observable<ApiResult<PagedResult<Customer>>> {
    return this.http.get<ApiResult<PagedResult<Customer>>>(this.endpoint, { params: createHttpParams(query) });
  }

  getById(id: string): Observable<ApiResult<Customer>> { return this.http.get<ApiResult<Customer>>(`${this.endpoint}/${id}`); }
  create(request: CustomerRequest): Observable<ApiResult<Customer>> { return this.http.post<ApiResult<Customer>>(this.endpoint, request); }
  import(file: File): Observable<ApiResult<CustomerImportResult>> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<ApiResult<CustomerImportResult>>(`${this.endpoint}/import`, formData);
  }
  downloadImportTemplate(): Observable<Blob> {
    return this.http.get(`${this.endpoint}/import-template`, { responseType: 'blob' });
  }
  update(id: string, request: CustomerRequest): Observable<ApiResult<Customer>> { return this.http.put<ApiResult<Customer>>(`${this.endpoint}/${id}`, request); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.endpoint}/${id}`); }
}
