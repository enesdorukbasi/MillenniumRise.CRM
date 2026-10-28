import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/api-error';
import { PagedResult } from '../../../../core/models/api.models';
import { CustomerProduct, DashboardSummary, Interaction } from '../../../../core/models/crm.models';
import { CustomerProductStatus, customerProductStatusLabels, interactionChannelLabels } from '../../../../core/models/domain.enums';
import { IconName } from '../../../../shared/components/icon/icon.component';
import { DashboardService } from '../../data-access/dashboard.service';

interface SummaryCard { label: string; value: number; icon: IconName; urgent?: boolean; }

@Component({ selector: 'app-dashboard-page', templateUrl: './dashboard-page.component.html', styleUrl: './dashboard-page.component.css', standalone: false })
export class DashboardPageComponent implements OnInit {
  loading = true;
  errorMessage = '';
  summary: DashboardSummary | null = null;
  followUps: PagedResult<CustomerProduct> | null = null;
  interactions: PagedResult<Interaction> | null = null;
  followUpPage = 1;
  interactionPage = 1;
  readonly pageSize = 10;
  readonly statusLabels = customerProductStatusLabels;
  readonly channelLabels = interactionChannelLabels;

  constructor(private readonly dashboardService: DashboardService) {}

  ngOnInit(): void { this.load(); }

  get summaryCards(): SummaryCard[] {
    return [
      { label: 'Toplam Müşteri', value: this.summary?.totalCustomerCount ?? 0, icon: 'customers' },
      { label: 'Aktif Ürün', value: this.summary?.activeProductCount ?? 0, icon: 'products' },
      { label: 'Müşteri Ürünü', value: this.summary?.customerProductCount ?? 0, icon: 'customer-products' },
      { label: 'Bekleyen Takip', value: this.summary?.pendingFollowUpCount ?? 0, icon: 'calendar', urgent: true }
    ];
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    forkJoin({
      summary: this.dashboardService.getSummary(),
      followUps: this.dashboardService.getUpcomingFollowUps(this.followUpPage, this.pageSize),
      interactions: this.dashboardService.getRecentInteractions(this.interactionPage, this.pageSize)
    }).subscribe({
      next: result => {
        this.summary = result.summary.data ?? null;
        this.followUps = result.followUps.data ?? null;
        this.interactions = result.interactions.data ?? null;
        this.loading = false;
      },
      error: error => { this.errorMessage = getApiErrorMessage(error); this.loading = false; }
    });
  }

  changeFollowUpPage(page: number): void { this.followUpPage = page; this.load(); }
  changeInteractionPage(page: number): void { this.interactionPage = page; this.load(); }

  statusClass(status: CustomerProductStatus): string {
    if (status === CustomerProductStatus.Successful) return 'status--success';
    if (status === CustomerProductStatus.Unsuccessful) return 'status--danger';
    if (status === CustomerProductStatus.FollowUpRequired || status === CustomerProductStatus.OnHold) return 'status--warning';
    return 'status--info';
  }
}
