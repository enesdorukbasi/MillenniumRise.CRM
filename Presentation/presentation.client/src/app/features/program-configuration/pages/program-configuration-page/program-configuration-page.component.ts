import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin, of, switchMap } from 'rxjs';
import { ProgramConfigurationService } from '../../../../core/configuration/program-configuration.service';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { ApiResult } from '../../../../core/models/api.models';
import { ProgramConfiguration, UploadedFile } from '../../../../core/models/crm.models';
import { UploadedFilesService } from '../../../files/data-access/uploaded-files.service';

@Component({
  selector: 'app-program-configuration-page',
  templateUrl: './program-configuration-page.component.html',
  styleUrl: './program-configuration-page.component.css',
  standalone: false
})
export class ProgramConfigurationPageComponent implements OnInit, OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  logoFile: File | null = null;
  authPageImageFile: File | null = null;
  logoPreviewUrl: string | null = null;
  authPageImagePreviewUrl: string | null = null;
  logoId: string | null = null;
  authPageImageId: string | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]]
  });

  constructor(
    readonly programConfigurationService: ProgramConfigurationService,
    private readonly uploadedFilesService: UploadedFilesService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  ngOnDestroy(): void {
    if (this.logoPreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.logoPreviewUrl);
    if (this.authPageImagePreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.authPageImagePreviewUrl);
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.programConfigurationService.get().pipe(finalize(() => this.loading = false)).subscribe({
      next: response => {
        if (!response.data) {
          this.errorMessage = 'Program ayarları alınamadı.';
          return;
        }

        if (this.logoPreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.logoPreviewUrl);
        if (this.authPageImagePreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.authPageImagePreviewUrl);
        this.form.setValue({ name: response.data.name });
        this.logoId = response.data.logoId;
        this.authPageImageId = response.data.authPageImageId;
        this.logoPreviewUrl = response.data.logo?.url ?? null;
        this.authPageImagePreviewUrl = response.data.authPageImage?.url ?? null;
        this.logoFile = null;
        this.authPageImageFile = null;
      },
      error: error => this.errorMessage = getApiErrorMessage(error)
    });
  }

  selectLogo(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    input.value = '';
    if (!file) return;
    if (!file.type.startsWith('image/')) {
      this.errorMessage = 'Logo için bir görsel dosyası seçmelisiniz.';
      return;
    }
    if (this.logoPreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.logoPreviewUrl);
    this.logoFile = file;
    this.logoPreviewUrl = URL.createObjectURL(file);
    this.errorMessage = '';
    this.successMessage = '';
  }

  selectAuthPageImage(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    input.value = '';
    if (!file) return;
    if (!file.type.startsWith('image/')) {
      this.errorMessage = 'Giriş sayfası için bir görsel dosyası seçmelisiniz.';
      return;
    }
    if (this.authPageImagePreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.authPageImagePreviewUrl);
    this.authPageImageFile = file;
    this.authPageImagePreviewUrl = URL.createObjectURL(file);
    this.errorMessage = '';
    this.successMessage = '';
  }

  removeLogo(): void {
    if (this.logoPreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.logoPreviewUrl);
    this.logoFile = null;
    this.logoId = null;
    this.logoPreviewUrl = null;
    this.successMessage = '';
  }

  removeAuthPageImage(): void {
    if (this.authPageImagePreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.authPageImagePreviewUrl);
    this.authPageImageFile = null;
    this.authPageImageId = null;
    this.authPageImagePreviewUrl = null;
    this.successMessage = '';
  }

  save(): void {
    if (this.form.invalid || this.saving) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const logoUpload = this.logoFile
      ? this.uploadedFilesService.upload(this.logoFile)
      : of<ApiResult<UploadedFile> | null>(null);
    const authPageImageUpload = this.authPageImageFile
      ? this.uploadedFilesService.upload(this.authPageImageFile)
      : of<ApiResult<UploadedFile> | null>(null);

    forkJoin({ logoUpload, authPageImageUpload }).pipe(
      switchMap(result => this.programConfigurationService.update({
        name: this.form.getRawValue().name,
        logoId: result.logoUpload?.data?.id ?? this.logoId,
        authPageImageId: result.authPageImageUpload?.data?.id ?? this.authPageImageId
      })),
      finalize(() => this.saving = false)
    ).subscribe({
      next: response => {
        if (!response.data) {
          this.errorMessage = 'Program ayarları güncellenemedi.';
          return;
        }

        if (this.logoPreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.logoPreviewUrl);
        if (this.authPageImagePreviewUrl?.startsWith('blob:')) URL.revokeObjectURL(this.authPageImagePreviewUrl);
        this.logoId = response.data.logoId;
        this.authPageImageId = response.data.authPageImageId;
        this.logoPreviewUrl = response.data.logo?.url ?? null;
        this.authPageImagePreviewUrl = response.data.authPageImage?.url ?? null;
        this.logoFile = null;
        this.authPageImageFile = null;
        this.form.markAsPristine();
        this.successMessage = 'Program ayarları güncellendi.';
      },
      error: error => this.errorMessage = getApiErrorMessage(error)
    });
  }
}
