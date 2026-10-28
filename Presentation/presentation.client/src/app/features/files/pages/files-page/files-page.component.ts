import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { UploadedFile, UploadedFileListQuery, UploadedFileSortField } from '../../../../core/models/crm.models';
import { SortingDirection } from '../../../../core/models/domain.enums';
import { UploadedFilesService } from '../../data-access/uploaded-files.service';

@Component({
  selector: 'app-files-page',
  templateUrl: './files-page.component.html',
  styleUrl: './files-page.component.css',
  standalone: false
})
export class FilesPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  loading = true;
  saving = false;
  errorMessage = '';
  formError = '';
  result: PagedResult<UploadedFile> | null = null;
  uploadOpen = false;
  deleteOpen = false;
  selectedFiles: File[] = [];
  fileToDelete: UploadedFile | null = null;
  query: UploadedFileListQuery = {
    pageNumber: 1,
    pageSize: 20,
    sortBy: UploadedFileSortField.CreatedAt,
    sortDirection: SortingDirection.Descending
  };
  readonly sortFields = UploadedFileSortField;
  readonly sortDirections = SortingDirection;
  readonly filterForm = this.formBuilder.group({
    search: [''],
    extension: [''],
    contentType: [''],
    sortBy: [UploadedFileSortField.CreatedAt],
    sortDirection: [SortingDirection.Descending]
  });

  constructor(private readonly uploadedFilesService: UploadedFilesService) {}
  get hasActiveFilters(): boolean { return !!this.query.search || !!this.query.extension || !!this.query.contentType; }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.uploadedFilesService.getList(this.query).subscribe({
      next: response => { this.result = response.data ?? null; this.loading = false; },
      error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; }
    });
  }

  applyFilters(): void {
    const value = this.filterForm.getRawValue();
    this.query = {
      ...this.query,
      search: value.search || undefined,
      extension: value.extension || undefined,
      contentType: value.contentType || undefined,
      sortBy: value.sortBy ?? UploadedFileSortField.CreatedAt,
      sortDirection: value.sortDirection ?? SortingDirection.Descending,
      pageNumber: 1
    };
    this.load();
  }

  clearFilters(): void {
    this.filterForm.patchValue({ search: '', extension: '', contentType: '' });
    this.query = { ...this.query, search: undefined, extension: undefined, contentType: undefined, pageNumber: 1 };
    this.load();
  }

  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.load(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.load(); }

  openUpload(): void {
    this.selectedFiles = [];
    this.formError = '';
    this.uploadOpen = true;
  }

  selectFiles(event: Event): void {
    this.selectedFiles = Array.from((event.target as HTMLInputElement).files ?? []);
  }

  upload(): void {
    if (!this.selectedFiles.length || this.saving) return;
    this.saving = true;
    this.formError = '';
    const operation: Observable<unknown> = this.selectedFiles.length === 1
      ? this.uploadedFilesService.upload(this.selectedFiles[0])
      : this.uploadedFilesService.uploadMany(this.selectedFiles);
    operation.pipe(finalize(() => this.saving = false)).subscribe({
      next: () => { this.uploadOpen = false; this.load(); },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  askDelete(file: UploadedFile): void { this.fileToDelete = file; this.formError = ''; this.deleteOpen = true; }

  confirmDelete(): void {
    if (!this.fileToDelete || this.saving) return;
    this.saving = true;
    this.formError = '';
    this.uploadedFilesService.delete(this.fileToDelete.id).pipe(finalize(() => this.saving = false)).subscribe({
      next: () => { this.deleteOpen = false; this.fileToDelete = null; this.load(); },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1_048_576) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / 1_048_576).toFixed(1)} MB`;
  }
}
