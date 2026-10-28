import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({ selector: 'app-pagination', templateUrl: './pagination.component.html', styleUrl: './pagination.component.css', standalone: false })
export class PaginationComponent {
  @Input() pageNumber = 1;
  @Input() pageSize = 20;
  @Input() totalCount = 0;
  @Input() totalPages = 0;
  @Output() readonly pageChange = new EventEmitter<number>();
  @Output() readonly pageSizeChange = new EventEmitter<number>();

  previous(): void { if (this.pageNumber > 1) this.pageChange.emit(this.pageNumber - 1); }
  next(): void { if (this.pageNumber < this.totalPages) this.pageChange.emit(this.pageNumber + 1); }
}
