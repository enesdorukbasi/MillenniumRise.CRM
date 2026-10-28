import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { CustomerProduct, Interaction, InteractionListQuery, InteractionRequest, InteractionSortField } from '../../../../core/models/crm.models';
import { InteractionChannel, InteractionOutcome, InteractionStatus, SortingDirection, interactionChannelLabels, interactionOutcomeLabels, interactionStatusLabels } from '../../../../core/models/domain.enums';
import { CustomerProductsService } from '../../../customer-products/data-access/customer-products.service';
import { InteractionsService } from '../../data-access/interactions.service';

@Component({ selector: 'app-interactions-page', templateUrl: './interactions-page.component.html', styleUrl: './interactions-page.component.css', standalone: false })
export class InteractionsPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  loading = true; saving = false; errorMessage = ''; formError = ''; result: PagedResult<Interaction> | null = null;
  formOpen = false; deleteOpen = false; selectedItem: Interaction | null = null; itemToDelete: Interaction | null = null; customerProducts: CustomerProduct[] = [];
  query: InteractionListQuery = { pageNumber: 1, pageSize: 20, sortBy: InteractionSortField.InteractionDate, sortDirection: SortingDirection.Descending };
  readonly channels = [InteractionChannel.Phone, InteractionChannel.Email, InteractionChannel.WhatsApp, InteractionChannel.SocialMedia, InteractionChannel.OnlineMeeting, InteractionChannel.InPersonMeeting, InteractionChannel.Other];
  readonly statuses = [InteractionStatus.Planned, InteractionStatus.Completed, InteractionStatus.NoResponse, InteractionStatus.Cancelled];
  readonly outcomes = [InteractionOutcome.Positive, InteractionOutcome.Neutral, InteractionOutcome.Negative, InteractionOutcome.FollowUpRequired];
  readonly channelLabels = interactionChannelLabels; readonly statusLabels = interactionStatusLabels; readonly outcomeLabels = interactionOutcomeLabels;
  readonly filterForm = this.formBuilder.group({ search: [''], customerProductId: [''], status: [null as InteractionStatus | null], channel: [null as InteractionChannel | null] });
  readonly interactionForm = this.formBuilder.group({
    customerProductId: this.formBuilder.nonNullable.control('', Validators.required),
    scheduledAt: this.formBuilder.nonNullable.control(''),
    occurredAt: this.formBuilder.nonNullable.control(''),
    channel: this.formBuilder.nonNullable.control(InteractionChannel.Phone, Validators.required),
    status: this.formBuilder.nonNullable.control(InteractionStatus.Planned, Validators.required),
    outcome: this.formBuilder.control<InteractionOutcome | null>(null),
    notes: this.formBuilder.nonNullable.control('', [Validators.required, Validators.maxLength(4000)]),
    nextFollowUpAt: this.formBuilder.nonNullable.control('')
  });

  constructor(private readonly interactionsService: InteractionsService, private readonly customerProductsService: CustomerProductsService) {}
  ngOnInit(): void { this.loadLookups(); this.load(); }
  loadLookups(): void { this.customerProductsService.getList({ pageNumber: 1, pageSize: 100 }).subscribe({ next: response => this.customerProducts = response.data?.items ?? [], error: () => undefined }); }
  load(): void { this.loading = true; this.errorMessage = ''; this.interactionsService.getList(this.query).subscribe({ next: response => { this.result = response.data ?? null; this.loading = false; }, error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; } }); }
  applyFilters(): void { const value = this.filterForm.getRawValue(); this.query = { ...this.query, search: value.search || undefined, customerProductId: value.customerProductId || undefined, status: value.status ?? undefined, channel: value.channel ?? undefined, pageNumber: 1 }; this.load(); }
  changePage(pageNumber: number): void { this.query = { ...this.query, pageNumber }; this.load(); }
  changePageSize(pageSize: number): void { this.query = { ...this.query, pageSize, pageNumber: 1 }; this.load(); }
  openCreate(): void { this.selectedItem = null; this.formError = ''; this.interactionForm.reset({ customerProductId: '', scheduledAt: '', occurredAt: '', channel: InteractionChannel.Phone, status: InteractionStatus.Planned, outcome: null, notes: '', nextFollowUpAt: '' }); this.formOpen = true; }
  openEdit(item: Interaction): void { this.selectedItem = item; this.formError = ''; this.interactionForm.reset({ customerProductId: item.customerProductId, scheduledAt: this.toDateTimeLocal(item.scheduledAt), occurredAt: this.toDateTimeLocal(item.occurredAt), channel: item.channel, status: item.status, outcome: item.outcome, notes: item.notes, nextFollowUpAt: this.toDateTimeLocal(item.nextFollowUpAt) }); this.formOpen = true; }
  save(): void {
    if (this.interactionForm.invalid || this.saving) { this.interactionForm.markAllAsTouched(); return; }
    const value = this.interactionForm.getRawValue();
    const request: InteractionRequest = { customerProductId: value.customerProductId, scheduledAt: this.toIso(value.scheduledAt), occurredAt: this.toIso(value.occurredAt), channel: value.channel, status: value.status, outcome: value.outcome, notes: value.notes, nextFollowUpAt: this.toIso(value.nextFollowUpAt) };
    this.saving = true; this.formError = ''; const operation = this.selectedItem ? this.interactionsService.update(this.selectedItem.id, request) : this.interactionsService.create(request);
    operation.pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.formOpen = false; this.load(); }, error: error => this.formError = getApiErrorMessage(error) });
  }
  askDelete(item: Interaction): void { this.itemToDelete = item; this.deleteOpen = true; }
  confirmDelete(): void { if (!this.itemToDelete || this.saving) return; this.saving = true; this.formError = ''; this.interactionsService.delete(this.itemToDelete.id).pipe(finalize(() => this.saving = false)).subscribe({ next: () => { this.deleteOpen = false; this.itemToDelete = null; this.load(); }, error: error => this.formError = getApiErrorMessage(error) }); }
  statusClass(status: InteractionStatus): string { if (status === InteractionStatus.Completed) return 'status--success'; if (status === InteractionStatus.Cancelled) return 'status--danger'; if (status === InteractionStatus.NoResponse) return 'status--warning'; return 'status--info'; }
  private toIso(value: string): string | null { return value ? new Date(value).toISOString() : null; }
  private toDateTimeLocal(value: string | null): string { if (!value) return ''; const date = new Date(value); const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000); return local.toISOString().slice(0, 16); }
}
