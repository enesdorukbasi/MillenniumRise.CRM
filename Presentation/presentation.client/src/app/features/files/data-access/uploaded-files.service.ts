import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createHttpParams } from '../../../core/http/http-params';
import { ApiResult, PagedResult } from '../../../core/models/api.models';
import { UploadedFile, UploadedFileListQuery } from '../../../core/models/crm.models';

@Injectable({ providedIn: 'root' })
export class UploadedFilesService {
  private readonly endpoint = '/api/UploadedFile';
  constructor(private readonly http: HttpClient) {}

  getList(query: UploadedFileListQuery): Observable<ApiResult<PagedResult<UploadedFile>>> {
    return this.http.get<ApiResult<PagedResult<UploadedFile>>>(this.endpoint, { params: createHttpParams(query) });
  }

  upload(file: File): Observable<ApiResult<UploadedFile>> {
    const formData = new FormData(); formData.append('File', file);
    return this.http.post<ApiResult<UploadedFile>>(this.endpoint, formData);
  }

  uploadMany(files: File[]): Observable<ApiResult<UploadedFile[]>> {
    const formData = new FormData(); files.forEach(file => formData.append('Files', file));
    return this.http.post<ApiResult<UploadedFile[]>>(`${this.endpoint}/batch`, formData);
  }

  delete(id: string): Observable<ApiResult<string>> { return this.http.delete<ApiResult<string>>(`${this.endpoint}/${id}`); }
}
