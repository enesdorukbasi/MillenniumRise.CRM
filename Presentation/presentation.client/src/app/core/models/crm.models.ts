import { BaseDto } from './api.models';
import { CustomerProductStatus, CustomerType, InteractionChannel, InteractionOutcome, InteractionStatus, SortingDirection } from './domain.enums';

export interface Customer extends BaseDto {
  fullName: string;
  companyName: string | null;
  phoneNumber: string | null;
  email: string | null;
  customerType: CustomerType;
  sector: string | null;
  website: string | null;
  sourceUrl: string | null;
  location: string | null;
  note: string | null;
}

export interface CustomerRequest {
  fullName: string;
  companyName: string | null;
  phoneNumber: string | null;
  email: string | null;
  customerType: CustomerType;
  sector: string | null;
  website: string | null;
  sourceUrl: string | null;
  location: string | null;
  note: string | null;
}

export enum CustomerSortField { FullName, CompanyName, CustomerType, CreatedAt, UpdatedAt }

export interface CustomerListQuery {
  search?: string;
  customerType?: CustomerType;
  productId?: string;
  hasProduct?: boolean;
  createdFrom?: string;
  createdTo?: string;
  sortBy?: CustomerSortField;
  sortDirection?: SortingDirection;
  pageNumber: number;
  pageSize: number;
}

export interface UploadedFile extends BaseDto {
  name: string;
  originalFileName: string;
  extension: string;
  url: string;
  contentType: string;
  fileSize: number;
  width: number | null;
  height: number | null;
  imageVariantsJson: string | null;
}

export enum UploadedFileSortField { Name, OriginalFileName, Extension, ContentType, FileSize, CreatedAt, UpdatedAt }

export interface UploadedFileListQuery {
  search?: string;
  extension?: string;
  contentType?: string;
  minimumFileSize?: number;
  maximumFileSize?: number;
  createdFrom?: string;
  createdTo?: string;
  sortBy?: UploadedFileSortField;
  sortDirection?: SortingDirection;
  pageNumber: number;
  pageSize: number;
}

export interface Product extends BaseDto {
  name: string;
  description: string | null;
  isActive: boolean;
  productLogoId: string | null;
  productLogo: UploadedFile | null;
}

export interface ProductRequest {
  name: string;
  description: string | null;
  isActive: boolean;
  productLogoId: string | null;
}

export enum ProductSortField { Name, IsActive, CreatedAt, UpdatedAt }

export interface ProductListQuery {
  search?: string;
  isActive?: boolean;
  createdFrom?: string;
  createdTo?: string;
  sortBy?: ProductSortField;
  sortDirection?: SortingDirection;
  pageNumber: number;
  pageSize: number;
}

export interface CustomerProduct extends BaseDto {
  customerId: string;
  customer: Customer;
  productId: string;
  product: Product;
  status: CustomerProductStatus;
  nextFollowUpAt: string | null;
  statusNote: string | null;
}

export interface CustomerProductRequest {
  customerId: string;
  productId: string;
  status: CustomerProductStatus;
  nextFollowUpAt: string | null;
  statusNote: string | null;
}

export enum CustomerProductSortField { CustomerName, ProductName, Status, NextFollowUpAt, CreatedAt, UpdatedAt }

export interface CustomerProductListQuery {
  search?: string;
  customerId?: string;
  productId?: string;
  status?: CustomerProductStatus;
  nextFollowUpFrom?: string;
  nextFollowUpTo?: string;
  sortBy?: CustomerProductSortField;
  sortDirection?: SortingDirection;
  pageNumber: number;
  pageSize: number;
}

export interface Interaction extends BaseDto {
  customerProductId: string;
  customerProduct: CustomerProduct;
  scheduledAt: string | null;
  occurredAt: string | null;
  channel: InteractionChannel;
  status: InteractionStatus;
  outcome: InteractionOutcome | null;
  notes: string;
  nextFollowUpAt: string | null;
}

export interface InteractionRequest {
  customerProductId: string;
  scheduledAt: string | null;
  occurredAt: string | null;
  channel: InteractionChannel;
  status: InteractionStatus;
  outcome: InteractionOutcome | null;
  notes: string;
  nextFollowUpAt: string | null;
}

export enum InteractionSortField { InteractionDate, ScheduledAt, OccurredAt, Status, Channel, NextFollowUpAt, CreatedAt, UpdatedAt }

export interface InteractionListQuery {
  search?: string;
  customerProductId?: string;
  status?: InteractionStatus;
  channel?: InteractionChannel;
  outcome?: InteractionOutcome;
  startDate?: string;
  endDate?: string;
  sortBy?: InteractionSortField;
  sortDirection?: SortingDirection;
  pageNumber: number;
  pageSize: number;
}

export interface DashboardSummary {
  totalCustomerCount: number;
  activeProductCount: number;
  customerProductCount: number;
  pendingFollowUpCount: number;
}
