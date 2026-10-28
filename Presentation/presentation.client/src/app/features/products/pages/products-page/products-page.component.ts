import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize, switchMap } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { Product, ProductListQuery, ProductRequest, ProductSortField } from '../../../../core/models/crm.models';
import { SortingDirection } from '../../../../core/models/domain.enums';
import { UploadedFilesService } from '../../../files/data-access/uploaded-files.service';
import { ProductsService } from '../../data-access/products.service';

@Component({ selector: 'app-products-page', templateUrl: './products-page.component.html', styleUrl: './products-page.component.css', standalone: false })
export class ProductsPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  loading = true; saving = false; errorMessage = ''; formError = ''; result: PagedResult<Product> | null = null;
  formOpen = false; deleteOpen = false; selectedProduct: Product | null = null; productToDelete: Product | null = null; logoFile: File | null = null;
  query: ProductListQuery = { pageNumber: 1, pageSize: 20, sortBy: ProductSortField.Name, sortDirection: SortingDirection.Ascending };
  readonly sortFields = ProductSortField; readonly sortDirections = SortingDirection;
  readonly filterForm = this.formBuilder.group({ search: [''], isActive: [null as boolean | null], sortBy: [ProductSortField.Name], sortDirection: [SortingDirection.Ascending] });
  readonly productForm = this.formBuilder.nonNullable.group({ name: ['', [Validators.required, Validators.maxLength(200)]], description: [''], isActive: [true] });

  constructor(private readonly productsService: ProductsService, private readonly uploadedFilesService: UploadedFilesService) {}
  ngOnInit(): void { this.load(); }
  load(): void { this.loading = true; this.errorMessage = ''; this.productsService.getList(this.query).subscribe({ next: response => { this.result = response.data ?? null; this.loading = false; }, error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; } }); }
  applyFilters(): void { const value = this.filterForm.getRawValue(); this.query = { ...this.query, search: value.search || undefined, isActive: value.isActive ?? undefined, sortBy: value.sortBy ?? ProductSortField.Name, sortDirection: value.sortDirection ?? SortingDirection.Ascending, pageNumber: 1 }; this.load(); }
  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.load(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.load(); }
  openCreate(): void { this.selectedProduct = null; this.logoFile = null; this.formError = ''; this.productForm.reset({ name: '', description: '', isActive: true }); this.formOpen = true; }
  openEdit(product: Product): void { this.selectedProduct = product; this.logoFile = null; this.formError = ''; this.productForm.reset({ name: product.name, description: product.description ?? '', isActive: product.isActive }); this.formOpen = true; }
  selectLogo(event: Event): void { this.logoFile = (event.target as HTMLInputElement).files?.[0] ?? null; }
  save(): void {
    if (this.productForm.invalid || this.saving) { this.productForm.markAllAsTouched(); return; }
    const value = this.productForm.getRawValue(); this.saving = true; this.formError = '';
    const saveProduct = (productLogoId: string | null) => {
      const request: ProductRequest = { name: value.name, description: value.description || null, isActive: value.isActive, productLogoId };
      return this.selectedProduct ? this.productsService.update(this.selectedProduct.id, request) : this.productsService.create(request);
    };
    const operation = this.logoFile
      ? this.uploadedFilesService.upload(this.logoFile).pipe(switchMap(response => saveProduct(response.data?.id ?? null)))
      : saveProduct(this.selectedProduct?.productLogoId ?? null);
    operation.pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.formOpen = false; this.load(); }, error: error => this.formError = getApiErrorMessage(error) });
  }
  askDelete(product: Product): void { this.productToDelete = product; this.deleteOpen = true; }
  confirmDelete(): void { if (!this.productToDelete || this.saving) return; this.saving = true; this.formError = ''; this.productsService.delete(this.productToDelete.id).pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.deleteOpen = false; this.productToDelete = null; this.load(); }, error: error => this.formError = getApiErrorMessage(error) }); }
}
