import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import {
  Customer,
  CustomerProduct,
  CustomerProductListQuery,
  CustomerProductRequest,
  CustomerProductSortField,
  Interaction,
  InteractionRequest,
  InteractionSortField,
  Product,
  ProductRequest
} from '../../../../core/models/crm.models';
import {
  CustomerProductStatus,
  InteractionChannel,
  InteractionOutcome,
  InteractionStatus,
  SortingDirection,
  customerProductStatusLabels,
  interactionChannelLabels,
  interactionChannelIconClasses,
  interactionChannelMarkerClasses,
  interactionOutcomeLabels,
  interactionStatusLabels
} from '../../../../core/models/domain.enums';
import { CustomerProductsService } from '../../../customer-products/data-access/customer-products.service';
import { CustomersService } from '../../../customers/data-access/customers.service';
import { InteractionsService } from '../../../interactions/data-access/interactions.service';
import { ProductsService } from '../../data-access/products.service';

@Component({
  selector: 'app-product-detail-page',
  templateUrl: './product-detail-page.component.html',
  styleUrl: './product-detail-page.component.css',
  standalone: false
})
export class ProductDetailPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly productId = this.route.snapshot.paramMap.get('id') ?? '';

  product: Product | null = null;
  result: PagedResult<CustomerProduct> | null = null;
  customerOptions: Customer[] = [];
  selectedCustomerProduct: CustomerProduct | null = null;
  expandedItem: CustomerProduct | null = null;
  expandedInteractions: PagedResult<Interaction> | null = null;
  interactionTarget: CustomerProduct | null = null;
  loading = true;
  tableLoading = true;
  expandedLoading = false;
  saving = false;
  errorMessage = '';
  tableError = '';
  formError = '';
  customerFormOpen = false;
  interactionFormOpen = false;
  productFormOpen = false;
  weeklyInteractionCount = 0;

  readonly statuses = [
    CustomerProductStatus.New,
    CustomerProductStatus.InProgress,
    CustomerProductStatus.FollowUpRequired,
    CustomerProductStatus.OnHold,
    CustomerProductStatus.Successful,
    CustomerProductStatus.Unsuccessful
  ];
  readonly channels = [
    InteractionChannel.Phone,
    InteractionChannel.Email,
    InteractionChannel.WhatsApp,
    InteractionChannel.SocialMedia,
    InteractionChannel.OnlineMeeting,
    InteractionChannel.InPersonMeeting,
    InteractionChannel.Other
  ];
  readonly interactionStatuses = [InteractionStatus.Planned, InteractionStatus.Completed, InteractionStatus.NoResponse, InteractionStatus.Cancelled];
  readonly outcomes = [InteractionOutcome.Positive, InteractionOutcome.Neutral, InteractionOutcome.Negative, InteractionOutcome.FollowUpRequired];
  readonly statusLabels = customerProductStatusLabels;
  readonly channelLabels = interactionChannelLabels;
  readonly channelIconClasses = interactionChannelIconClasses;
  readonly channelMarkerClasses = interactionChannelMarkerClasses;
  readonly interactionStatusLabels = interactionStatusLabels;
  readonly outcomeLabels = interactionOutcomeLabels;
  readonly sortFields = CustomerProductSortField;
  readonly sortDirections = SortingDirection;
  readonly statusCounts: Record<CustomerProductStatus, number> = {
    [CustomerProductStatus.New]: 0,
    [CustomerProductStatus.InProgress]: 0,
    [CustomerProductStatus.FollowUpRequired]: 0,
    [CustomerProductStatus.OnHold]: 0,
    [CustomerProductStatus.Successful]: 0,
    [CustomerProductStatus.Unsuccessful]: 0
  };
  readonly stageClasses: Record<CustomerProductStatus, string> = {
    [CustomerProductStatus.New]: 'pipeline-stage--new',
    [CustomerProductStatus.InProgress]: 'pipeline-stage--progress',
    [CustomerProductStatus.FollowUpRequired]: 'pipeline-stage--follow-up',
    [CustomerProductStatus.OnHold]: 'pipeline-stage--hold',
    [CustomerProductStatus.Successful]: 'pipeline-stage--success',
    [CustomerProductStatus.Unsuccessful]: 'pipeline-stage--danger'
  };

  query: CustomerProductListQuery = {
    productId: this.productId,
    pageNumber: 1,
    pageSize: 10,
    sortBy: CustomerProductSortField.CustomerName,
    sortDirection: SortingDirection.Ascending
  };

  readonly filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    sortBy: [CustomerProductSortField.CustomerName],
    sortDirection: [SortingDirection.Ascending]
  });
  readonly customerProductForm = this.formBuilder.nonNullable.group({
    customerId: ['', Validators.required],
    status: [CustomerProductStatus.New, Validators.required],
    nextFollowUpAt: [''],
    statusNote: ['']
  });
  readonly productForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    isActive: [true]
  });
  readonly interactionForm = this.formBuilder.group({
    scheduledAt: this.formBuilder.nonNullable.control(''),
    occurredAt: this.formBuilder.nonNullable.control(''),
    channel: this.formBuilder.nonNullable.control(InteractionChannel.Phone, Validators.required),
    status: this.formBuilder.nonNullable.control(InteractionStatus.Planned, Validators.required),
    outcome: this.formBuilder.control<InteractionOutcome | null>(null),
    notes: this.formBuilder.nonNullable.control('', [Validators.required, Validators.maxLength(4000)]),
    nextFollowUpAt: this.formBuilder.nonNullable.control('')
  });

  constructor(
    private readonly productsService: ProductsService,
    private readonly customerProductsService: CustomerProductsService,
    private readonly customersService: CustomersService,
    private readonly interactionsService: InteractionsService
  ) {}

  get totalCustomerCount(): number { return this.statuses.reduce((total, status) => total + this.statusCounts[status], 0); }
  get hasActiveFilters(): boolean {
    return !!this.query.search
      || this.query.sortBy !== CustomerProductSortField.CustomerName
      || this.query.sortDirection !== SortingDirection.Ascending;
  }

  ngOnInit(): void {
    this.loadProduct();
    this.loadCustomerProducts();
    this.loadSummary();
    this.loadCustomerOptions();
  }

  loadProduct(): void {
    this.loading = true;
    this.errorMessage = '';
    this.productsService.getById(this.productId).subscribe({
      next: response => { this.product = response.data ?? null; this.loading = false; },
      error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; }
    });
  }

  loadCustomerProducts(): void {
    this.tableLoading = true;
    this.tableError = '';
    this.expandedItem = null;
    this.expandedInteractions = null;
    this.customerProductsService.getList(this.query).subscribe({
      next: response => { this.result = response.data ?? null; this.tableLoading = false; },
      error: error => { this.tableError = getApiErrorMessage(error); this.tableLoading = false; }
    });
  }

  loadSummary(): void {
    const weekStart = new Date();
    weekStart.setDate(weekStart.getDate() - 7);
    const statusRequests = this.statuses.map(status => this.customerProductsService.getList({
      productId: this.productId,
      status,
      pageNumber: 1,
      pageSize: 1
    }));

    forkJoin({
      statusResponses: forkJoin(statusRequests),
      weeklyInteractions: this.interactionsService.getList({
        productId: this.productId,
        startDate: weekStart.toISOString(),
        pageNumber: 1,
        pageSize: 1
      })
    }).subscribe({
      next: response => {
        response.statusResponses.forEach((current, index) => this.statusCounts[this.statuses[index]] = current.data?.totalCount ?? 0);
        this.weeklyInteractionCount = response.weeklyInteractions.data?.totalCount ?? 0;
      },
      error: () => undefined
    });
  }

  loadCustomerOptions(): void {
    this.customersService.getList({ productId: this.productId, hasProduct: false, pageNumber: 1, pageSize: 100 }).subscribe({
      next: response => this.customerOptions = response.data?.items ?? [],
      error: () => this.customerOptions = []
    });
  }

  selectStatus(status: CustomerProductStatus | null): void {
    this.query = { ...this.query, status: status ?? undefined, pageNumber: 1 };
    this.loadCustomerProducts();
  }

  applyFilters(): void {
    const value = this.filterForm.getRawValue();
    this.query = {
      ...this.query,
      search: value.search || undefined,
      sortBy: value.sortBy,
      sortDirection: value.sortDirection,
      pageNumber: 1
    };
    this.loadCustomerProducts();
  }

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      sortBy: CustomerProductSortField.CustomerName,
      sortDirection: SortingDirection.Ascending
    });
    this.query = {
      ...this.query,
      search: undefined,
      sortBy: CustomerProductSortField.CustomerName,
      sortDirection: SortingDirection.Ascending,
      pageNumber: 1
    };
    this.loadCustomerProducts();
  }

  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.loadCustomerProducts(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.loadCustomerProducts(); }

  toggleInteractions(item: CustomerProduct): void {
    if (this.expandedItem?.id === item.id) {
      this.expandedItem = null;
      this.expandedInteractions = null;
      return;
    }
    this.expandedItem = item;
    this.loadExpandedInteractions(1);
  }

  loadExpandedInteractions(pageNumber: number): void {
    if (!this.expandedItem) return;
    this.expandedLoading = true;
    this.interactionsService.getList({
      customerProductId: this.expandedItem.id,
      pageNumber,
      pageSize: 4,
      sortBy: InteractionSortField.InteractionDate,
      sortDirection: SortingDirection.Descending
    }).subscribe({
      next: response => { this.expandedInteractions = response.data ?? null; this.expandedLoading = false; },
      error: () => { this.expandedInteractions = null; this.expandedLoading = false; }
    });
  }

  openAddCustomer(): void {
    this.selectedCustomerProduct = null;
    this.formError = '';
    this.customerProductForm.reset({ customerId: '', status: CustomerProductStatus.New, nextFollowUpAt: '', statusNote: '' });
    this.customerFormOpen = true;
  }

  openEditCustomerProduct(item: CustomerProduct): void {
    this.selectedCustomerProduct = item;
    this.formError = '';
    const nextFollowUpAt = item.nextFollowUpAt
      ? new Date(new Date(item.nextFollowUpAt).getTime() - new Date(item.nextFollowUpAt).getTimezoneOffset() * 60000).toISOString().slice(0, 16)
      : '';
    this.customerProductForm.reset({
      customerId: item.customerId,
      status: item.status,
      nextFollowUpAt,
      statusNote: item.statusNote ?? ''
    });
    this.customerFormOpen = true;
  }

  saveCustomerProduct(): void {
    if (this.customerProductForm.invalid || this.saving) { this.customerProductForm.markAllAsTouched(); return; }
    const value = this.customerProductForm.getRawValue();
    const request: CustomerProductRequest = {
      customerId: value.customerId,
      productId: this.productId,
      status: value.status,
      nextFollowUpAt: this.toIso(value.nextFollowUpAt),
      statusNote: value.statusNote || null
    };
    this.saving = true;
    this.formError = '';
    const operation = this.selectedCustomerProduct
      ? this.customerProductsService.update(this.selectedCustomerProduct.id, request)
      : this.customerProductsService.create(request);
    operation.pipe(finalize(() => this.saving = false)).subscribe({
      next: () => {
        this.customerFormOpen = false;
        this.selectedCustomerProduct = null;
        this.loadCustomerProducts();
        this.loadSummary();
        this.loadCustomerOptions();
      },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  openAddInteraction(item: CustomerProduct): void {
    this.interactionTarget = item;
    this.formError = '';
    this.interactionForm.reset({
      scheduledAt: '',
      occurredAt: '',
      channel: InteractionChannel.Phone,
      status: InteractionStatus.Planned,
      outcome: null,
      notes: '',
      nextFollowUpAt: ''
    });
    this.interactionFormOpen = true;
  }

  saveInteraction(): void {
    if (!this.interactionTarget || this.interactionForm.invalid || this.saving) { this.interactionForm.markAllAsTouched(); return; }
    const value = this.interactionForm.getRawValue();
    const request: InteractionRequest = {
      customerProductId: this.interactionTarget.id,
      scheduledAt: this.toIso(value.scheduledAt),
      occurredAt: this.toIso(value.occurredAt),
      channel: value.channel,
      status: value.status,
      outcome: value.outcome,
      notes: value.notes,
      nextFollowUpAt: this.toIso(value.nextFollowUpAt)
    };
    this.saving = true;
    this.formError = '';
    this.interactionsService.create(request).pipe(finalize(() => this.saving = false)).subscribe({
      next: response => {
        const interaction = response.data;
        const targetId = this.interactionTarget?.id;
        this.interactionFormOpen = false;
        if (interaction && targetId && this.result) {
          const updatedItems = this.result.items.map(item => item.id === targetId
            ? {
                ...item,
                lastInteractionAt: interaction.occurredAt || interaction.scheduledAt || interaction.createdAt,
                lastInteractionChannel: interaction.channel
              }
            : item);
          this.result = { ...this.result, items: updatedItems };
          if (this.expandedItem?.id === targetId) {
            this.expandedItem = updatedItems.find(item => item.id === targetId) ?? this.expandedItem;
            this.loadExpandedInteractions(1);
          }
        } else if (this.expandedItem?.id === targetId) {
          this.loadExpandedInteractions(1);
        }
        this.loadSummary();
      },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  openEditProduct(): void {
    if (!this.product) return;
    this.formError = '';
    this.productForm.reset({ name: this.product.name, description: this.product.description ?? '', isActive: this.product.isActive });
    this.productFormOpen = true;
  }

  saveProduct(): void {
    if (!this.product || this.productForm.invalid || this.saving) { this.productForm.markAllAsTouched(); return; }
    const value = this.productForm.getRawValue();
    const request: ProductRequest = {
      name: value.name,
      description: value.description || null,
      isActive: value.isActive,
      productLogoId: this.product.productLogoId
    };
    this.saving = true;
    this.formError = '';
    this.productsService.update(this.product.id, request).pipe(finalize(() => this.saving = false)).subscribe({
      next: response => { this.product = response.data ?? this.product; this.productFormOpen = false; },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  statusClass(status: CustomerProductStatus): string {
    if (status === CustomerProductStatus.Successful) return 'status--success';
    if (status === CustomerProductStatus.Unsuccessful) return 'status--danger';
    if (status === CustomerProductStatus.FollowUpRequired || status === CustomerProductStatus.OnHold) return 'status--warning';
    return 'status--info';
  }

  interactionStatusClass(status: InteractionStatus): string {
    if (status === InteractionStatus.Completed) return 'status--success';
    if (status === InteractionStatus.Cancelled) return 'status--danger';
    if (status === InteractionStatus.NoResponse) return 'status--warning';
    return 'status--info';
  }

  private toIso(value: string): string | null { return value ? new Date(value).toISOString() : null; }
}
