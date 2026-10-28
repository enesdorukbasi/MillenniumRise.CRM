import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-page-header',
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.css',
  standalone: false
})
export class PageHeaderComponent {
  @Input({ required: true }) title!: string;
  @Input() description = '';
  @Input() actionLabel = '';
  @Output() readonly action = new EventEmitter<void>();
}
