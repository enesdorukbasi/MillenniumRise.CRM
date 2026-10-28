import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../models/api.models';

export function getApiErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Beklenmeyen bir hata oluştu.';
  }

  const body = error.error as unknown;
  let problem: ProblemDetails | undefined;

  if (typeof body === 'object' && body !== null && 'fail' in body) {
    problem = (body as { fail?: ProblemDetails | null }).fail ?? undefined;
  } else if (typeof body === 'object' && body !== null) {
    problem = body as ProblemDetails;
  }

  if (problem?.errors?.length) {
    return problem.errors.join(' ');
  }

  return problem?.detail || problem?.title || 'İşlem tamamlanamadı. Lütfen tekrar deneyin.';
}
