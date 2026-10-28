import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({ selector: 'app-modal', templateUrl: './modal.component.html', styleUrl: './modal.component.css', standalone: false })
export class ModalComponent {
  @Input() open = false;
  @Input({ required: true }) title!: string;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Output() readonly closed = new EventEmitter<void>();

  @HostListener('document:keydown.escape')
  closeOnEscape(): void { if (this.open) this.closed.emit(); }
}
