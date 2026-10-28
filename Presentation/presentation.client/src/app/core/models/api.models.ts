export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: string[];
  [key: string]: unknown;
}

export interface ApiResult<T = never> {
  data?: T | null;
  fail?: ProblemDetails | null;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface BaseDto {
  id: string;
  createdAt: string;
  updatedAt: string | null;
}
