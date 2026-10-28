import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { CustomerProduct, CustomerProductRequest, Interaction, InteractionListQuery, InteractionRequest, InteractionSortField } from '../../../../core/models/crm.models';
import {
  CustomerProductStatus,
  CustomerType,
  InteractionChannel,
  InteractionOutcome,
  InteractionStatus,
  SortingDirection,
  customerProductStatusLabels,
  customerTypeLabels,
  interactionChannelIconClasses,
  interactionChannelLabels,
  interactionChannelMarkerClasses,
  interactionOutcomeLabels,
  interactionStatusLabels
} from '../../../../core/models/domain.enums';
import { CustomerProductsService } from '../../../customer-products/data-access/customer-products.service';
import { InteractionsService } from '../../../interactions/data-access/interactions.service';

@Component({
  selector: 'app-customer-product-detail-page',
  templateUrl: './customer-product-detail-page.component.html',
  styleUrl: './customer-product-detail-page.component.css',
  standalone: false
})
export class CustomerProductDetailPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  readonly productId = this.route.snapshot.paramMap.get('productId') ?? '';
  private readonly customerProductId = this.route.snapshot.paramMap.get('customerProductId') ?? '';

  item: CustomerProduct | null = null;
  result: PagedResult<Interaction> | null = null;
  loading = true;
  interactionsLoading = true;
  saving = false;
  errorMessage = '';
  interactionError = '';
  formError = '';
  interactionFormOpen = false;
  relationshipFormOpen = false;

  readonly statuses = [CustomerProductStatus.New, CustomerProductStatus.InProgress, CustomerProductStatus.FollowUpRequired, CustomerProductStatus.OnHold, CustomerProductStatus.Successful, CustomerProductStatus.Unsuccessful];
  readonly interactionStatuses = [InteractionStatus.Planned, InteractionStatus.Completed, InteractionStatus.NoResponse, InteractionStatus.Cancelled];
  readonly channels = [InteractionChannel.Phone, InteractionChannel.Email, InteractionChannel.WhatsApp, InteractionChannel.SocialMedia, InteractionChannel.OnlineMeeting, InteractionChannel.InPersonMeeting, InteractionChannel.Other];
  readonly outcomes = [InteractionOutcome.Positive, InteractionOutcome.Neutral, InteractionOutcome.Negative, InteractionOutcome.FollowUpRequired];
  readonly statusLabels = customerProductStatusLabels;
  readonly customerTypeLabels = customerTypeLabels;
  readonly interactionStatusLabels = interactionStatusLabels;
  readonly channelLabels = interactionChannelLabels;
  readonly channelIconClasses = interactionChannelIconClasses;
  readonly channelMarkerClasses = interactionChannelMarkerClasses;
  readonly outcomeLabels = interactionOutcomeLabels;

  query: InteractionListQuery = {
    customerProductId: this.customerProductId,
    pageNumber: 1,
    pageSize: 10,
    sortBy: InteractionSortField.InteractionDate,
    sortDirection: SortingDirection.Descending
  };

  readonly filterForm = this.formBuilder.group({
    search: [''],
    channel: [null as InteractionChannel | null]
  });
  readonly relationshipForm = this.formBuilder.nonNullable.group({
    status: [CustomerProductStatus.New, Validators.required],
    nextFollowUpAt: [''],
    statusNote: ['']
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
    private readonly customerProductsService: CustomerProductsService,
    private readonly interactionsService: InteractionsService
  ) {}

  get hasActiveFilters(): boolean { return !!this.query.search || this.query.channel !== undefined || this.query.status !== undefined; }

  ngOnInit(): void { this.loadPage(); }

  loadPage(): void {
    this.loading = true;
    this.interactionsLoading = true;
    this.errorMessage = '';
    forkJoin({
      item: this.customerProductsService.getById(this.customerProductId),
      interactions: this.interactionsService.getList(this.query)
    }).subscribe({
      next: response => {
        this.item = response.item.data ?? null;
        this.result = response.interactions.data ?? null;
        this.loading = false;
        this.interactionsLoading = false;
      },
      error: error => {
        this.errorMessage = getApiErrorMessage(error);
        this.loading = false;
        this.interactionsLoading = false;
      }
    });
  }

  loadInteractions(): void {
    this.interactionsLoading = true;
    this.interactionError = '';
    this.interactionsService.getList(this.query).subscribe({
      next: response => { this.result = response.data ?? null; this.interactionsLoading = false; },
      error: error => { this.interactionError = getApiErrorMessage(error); this.interactionsLoading = false; }
    });
  }

  selectInteractionStatus(status: InteractionStatus | null): void {
    this.query = { ...this.query, status: status ?? undefined, pageNumber: 1 };
    this.loadInteractions();
  }

  applyFilters(): void {
    const value = this.filterForm.getRawValue();
    this.query = { ...this.query, search: value.search || undefined, channel: value.channel ?? undefined, pageNumber: 1 };
    this.loadInteractions();
  }

  clearFilters(): void {
    this.filterForm.patchValue({ search: '', channel: null });
    this.query = { ...this.query, search: undefined, channel: undefined, status: undefined, pageNumber: 1 };
    this.loadInteractions();
  }

  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.loadInteractions(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.loadInteractions(); }

  openRelationshipEdit(): void {
    if (!this.item) return;
    this.formError = '';
    this.relationshipForm.reset({
      status: this.item.status,
      nextFollowUpAt: this.toDateTimeLocal(this.item.nextFollowUpAt),
      statusNote: this.item.statusNote ?? ''
    });
    this.relationshipFormOpen = true;
  }

  saveRelationship(): void {
    if (!this.item || this.relationshipForm.invalid || this.saving) return;
    const value = this.relationshipForm.getRawValue();
    const request: CustomerProductRequest = {
      customerId: this.item.customerId,
      productId: this.item.productId,
      status: value.status,
      nextFollowUpAt: this.toIso(value.nextFollowUpAt),
      statusNote: value.statusNote || null
    };
    this.saving = true;
    this.formError = '';
    this.customerProductsService.update(this.item.id, request).pipe(finalize(() => this.saving = false)).subscribe({
      next: response => { this.item = response.data ?? this.item; this.relationshipFormOpen = false; },
      error: error => this.formError = getApiErrorMessage(error)
    });
  }

  openAddInteraction(): void {
    this.formError = '';
    this.interactionForm.reset({ scheduledAt: '', occurredAt: '', channel: InteractionChannel.Phone, status: InteractionStatus.Planned, outcome: null, notes: '', nextFollowUpAt: '' });
    this.interactionFormOpen = true;
  }

  saveInteraction(): void {
    if (this.interactionForm.invalid || this.saving) { this.interactionForm.markAllAsTouched(); return; }
    const value = this.interactionForm.getRawValue();
    const request: InteractionRequest = {
      customerProductId: this.customerProductId,
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
        if (this.item && interaction) {
          this.item = {
            ...this.item,
            lastInteractionAt: interaction.occurredAt || interaction.scheduledAt || interaction.createdAt,
            lastInteractionChannel: interaction.channel
          };
        }
        this.interactionFormOpen = false;
        this.query = { ...this.query, pageNumber: 1 };
        this.loadInteractions();
      },
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

  interactionOutcomeClass(outcome: InteractionOutcome | null): string {
    if (outcome === InteractionOutcome.Positive) return 'status--success';
    if (outcome === InteractionOutcome.Negative) return 'status--danger';
    if (outcome === InteractionOutcome.FollowUpRequired) return 'status--warning';
    return 'status--neutral';
  }

  customerTypeLabel(type: CustomerType): string { return this.customerTypeLabels[type]; }
  private toIso(value: string): string | null { return value ? new Date(value).toISOString() : null; }
  private toDateTimeLocal(value: string | null): string { if (!value) return ''; const date = new Date(value); const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000); return local.toISOString().slice(0, 16); }
}
