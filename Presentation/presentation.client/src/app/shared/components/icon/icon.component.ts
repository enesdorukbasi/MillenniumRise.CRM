import { Component, Input } from '@angular/core';

export type IconName =
  | 'dashboard' | 'customers' | 'products' | 'customer-products' | 'interactions' | 'files'
  | 'menu' | 'logout' | 'calendar' | 'search' | 'plus' | 'edit' | 'trash' | 'close'
  | 'chevron-left' | 'chevron-right' | 'upload' | 'mail' | 'lock' | 'eye' | 'eye-off'
  | 'arrow-right' | 'refresh' | 'alert' | 'external' | 'settings' | 'image' | 'save' | 'chevron-down'
  | 'chevrons-left' | 'chevrons-right' | 'sun' | 'moon' | 'monitor' | 'check';

@Component({
  selector: 'app-icon',
  templateUrl: './icon.component.html',
  styleUrl: './icon.component.css',
  standalone: false
})
export class IconComponent {
  @Input({ required: true }) name!: IconName;
  @Input() size = 20;
}
