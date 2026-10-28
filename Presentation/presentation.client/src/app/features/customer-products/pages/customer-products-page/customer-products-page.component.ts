import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { Customer, CustomerProduct, CustomerProductListQuery, CustomerProductRequest, CustomerProductSortField, Product } from '../../../../core/models/crm.models';
import { CustomerProductStatus, SortingDirection, customerProductStatusLabels } from '../../../../core/models/domain.enums';
import { CustomersService } from '../../../customers/data-access/customers.service';
import { ProductsService } from '../../../products/data-access/products.service';
import { CustomerProductsService } from '../../data-access/customer-products.service';

@Component({ selector: 'app-customer-products-page', templateUrl: './customer-products-page.component.html', styleUrl: './customer-products-page.component.css', standalone: false })
export class CustomerProductsPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  loading = true; saving = false; errorMessage = ''; formError = ''; result: PagedResult<CustomerProduct> | null = null;
  formOpen = false; deleteOpen = false; selectedItem: CustomerProduct | null = null; itemToDelete: CustomerProduct | null = null;
  customers: Customer[] = []; products: Product[] = [];
  query: CustomerProductListQuery = { pageNumber: 1, pageSize: 20, sortBy: CustomerProductSortField.CustomerName, sortDirection: SortingDirection.Ascending };
  readonly statuses = [CustomerProductStatus.New, CustomerProductStatus.InProgress, CustomerProductStatus.FollowUpRequired, CustomerProductStatus.OnHold, CustomerProductStatus.Successful, CustomerProductStatus.Unsuccessful];
  readonly statusLabels = customerProductStatusLabels; readonly sortFields = CustomerProductSortField; readonly sortDirections = SortingDirection;
  readonly filterForm = this.formBuilder.group({ search: [''], customerId: [''], productId: [''], status: [null as CustomerProductStatus | null], sortBy: [CustomerProductSortField.CustomerName], sortDirection: [SortingDirection.Ascending] });
  readonly itemForm = this.formBuilder.nonNullable.group({ customerId: ['', Validators.required], productId: ['', Validators.required], status: [CustomerProductStatus.New, Validators.required], nextFollowUpAt: [''], statusNote: [''] });

  constructor(private readonly customerProductsService: CustomerProductsService, private readonly customersService: CustomersService, private readonly productsService: ProductsService) {}
  ngOnInit(): void { this.loadLookups(); this.load(); }
  loadLookups(): void {
    forkJoin({ customers: this.customersService.getList({ pageNumber: 1, pageSize: 100 }), products: this.productsService.getList({ pageNumber: 1, pageSize: 100 }) }).subscribe({
      next: response => { this.customers = response.customers.data?.items ?? []; this.products = response.products.data?.items ?? []; }, error: () => undefined
    });
  }
  load(): void { this.loading = true; this.errorMessage = ''; this.customerProductsService.getList(this.query).subscribe({ next: response => { this.result = response.data ?? null; this.loading = false; }, error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; } }); }
  applyFilters(): void { const value = this.filterForm.getRawValue(); this.query = { ...this.query, search: value.search || undefined, customerId: value.customerId || undefined, productId: value.productId || undefined, status: value.status ?? undefined, sortBy: value.sortBy ?? CustomerProductSortField.CustomerName, sortDirection: value.sortDirection ?? SortingDirection.Ascending, pageNumber: 1 }; this.load(); }
  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.load(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.load(); }
  openCreate(): void { this.selectedItem = null; this.formError = ''; this.itemForm.reset({ customerId: '', productId: '', status: CustomerProductStatus.New, nextFollowUpAt: '', statusNote: '' }); this.formOpen = true; }
  openEdit(item: CustomerProduct): void { this.selectedItem = item; this.formError = ''; this.itemForm.reset({ customerId: item.customerId, productId: item.productId, status: item.status, nextFollowUpAt: this.toDateTimeLocal(item.nextFollowUpAt), statusNote: item.statusNote ?? '' }); this.formOpen = true; }
  save(): void {
    if (this.itemForm.invalid || this.saving) { this.itemForm.markAllAsTouched(); return; }
    const value = this.itemForm.getRawValue(); const request: CustomerProductRequest = { customerId: value.customerId, productId: value.productId, status: value.status, nextFollowUpAt: value.nextFollowUpAt ? new Date(value.nextFollowUpAt).toISOString() : null, statusNote: value.statusNote || null };
    this.saving = true; this.formError = ''; const operation = this.selectedItem ? this.customerProductsService.update(this.selectedItem.id, request) : this.customerProductsService.create(request);
    operation.pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.formOpen = false; this.load(); }, error: error => this.formError = getApiErrorMessage(error) });
  }
  askDelete(item: CustomerProduct): void { this.itemToDelete = item; this.deleteOpen = true; }
  confirmDelete(): void { if (!this.itemToDelete || this.saving) return; this.saving = true; this.formError = ''; this.customerProductsService.delete(this.itemToDelete.id).pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.deleteOpen = false; this.itemToDelete = null; this.load(); }, error: error => this.formError = getApiErrorMessage(error) }); }
  statusClass(status: CustomerProductStatus): string { if (status === CustomerProductStatus.Successful) return 'status--success'; if (status === CustomerProductStatus.Unsuccessful) return 'status--danger'; if (status === CustomerProductStatus.FollowUpRequired || status === CustomerProductStatus.OnHold) return 'status--warning'; return 'status--info'; }
  private toDateTimeLocal(value: string | null): string { if (!value) return ''; const date = new Date(value); const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000); return local.toISOString().slice(0, 16); }
}
