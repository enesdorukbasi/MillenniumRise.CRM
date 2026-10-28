import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { Customer, CustomerListQuery, CustomerRequest, CustomerSortField } from '../../../../core/models/crm.models';
import { CustomerType, SortingDirection, customerTypeLabels } from '../../../../core/models/domain.enums';
import { CustomersService } from '../../data-access/customers.service';

@Component({ selector: 'app-customers-page', templateUrl: './customers-page.component.html', styleUrl: './customers-page.component.css', standalone: false })
export class CustomersPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  loading = true;
  saving = false;
  errorMessage = '';
  formError = '';
  result: PagedResult<Customer> | null = null;
  formOpen = false;
  deleteOpen = false;
  selectedCustomer: Customer | null = null;
  customerToDelete: Customer | null = null;
  query: CustomerListQuery = { pageNumber: 1, pageSize: 20, sortBy: CustomerSortField.FullName, sortDirection: SortingDirection.Ascending };
  readonly customerTypes = [CustomerType.Individual, CustomerType.Business];
  readonly customerTypeLabels = customerTypeLabels;
  readonly sortFields = CustomerSortField;
  readonly sortDirections = SortingDirection;

  readonly filterForm = this.formBuilder.group({ search: [''], customerType: [null as CustomerType | null], sortBy: [CustomerSortField.FullName], sortDirection: [SortingDirection.Ascending] });
  readonly customerForm = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    companyName: [''], phoneNumber: [''], email: ['', Validators.email], customerType: [CustomerType.Individual, Validators.required],
    sector: [''], website: [''], sourceUrl: [''], location: [''], note: ['']
  });

  constructor(private readonly customersService: CustomersService) {}
  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true; this.errorMessage = '';
    this.customersService.getList(this.query).subscribe({
      next: response => { this.result = response.data ?? null; this.loading = false; },
      error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; }
    });
  }

  applyFilters(): void {
    const value = this.filterForm.getRawValue();
    this.query = { ...this.query, search: value.search || undefined, customerType: value.customerType ?? undefined, sortBy: value.sortBy ?? CustomerSortField.FullName, sortDirection: value.sortDirection ?? SortingDirection.Ascending, pageNumber: 1 };
    this.load();
  }

  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.load(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.load(); }

  openCreate(): void {
    this.selectedCustomer = null; this.formError = ''; this.customerForm.reset({ fullName: '', companyName: '', phoneNumber: '', email: '', customerType: CustomerType.Individual, sector: '', website: '', sourceUrl: '', location: '', note: '' }); this.formOpen = true;
  }

  openEdit(customer: Customer): void {
    this.selectedCustomer = customer; this.formError = '';
    this.customerForm.reset({ fullName: customer.fullName, companyName: customer.companyName ?? '', phoneNumber: customer.phoneNumber ?? '', email: customer.email ?? '', customerType: customer.customerType, sector: customer.sector ?? '', website: customer.website ?? '', sourceUrl: customer.sourceUrl ?? '', location: customer.location ?? '', note: customer.note ?? '' });
    this.formOpen = true;
  }

  save(): void {
    if (this.customerForm.invalid || this.saving) { this.customerForm.markAllAsTouched(); return; }
    const value = this.customerForm.getRawValue();
    const request: CustomerRequest = { fullName: value.fullName, companyName: value.companyName || null, phoneNumber: value.phoneNumber || null, email: value.email || null, customerType: value.customerType, sector: value.sector || null, website: value.website || null, sourceUrl: value.sourceUrl || null, location: value.location || null, note: value.note || null };
    this.saving = true; this.formError = '';
    const operation = this.selectedCustomer ? this.customersService.update(this.selectedCustomer.id, request) : this.customersService.create(request);
    operation.pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.formOpen = false; this.load(); }, error: error => this.formError = getApiErrorMessage(error) });
  }

  askDelete(customer: Customer): void { this.customerToDelete = customer; this.deleteOpen = true; }
  confirmDelete(): void {
    if (!this.customerToDelete || this.saving) return;
    this.saving = true; this.formError = '';
    this.customersService.delete(this.customerToDelete.id).pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.deleteOpen = false; this.customerToDelete = null; this.load(); }, error: error => this.formError = getApiErrorMessage(error) });
  }
}
