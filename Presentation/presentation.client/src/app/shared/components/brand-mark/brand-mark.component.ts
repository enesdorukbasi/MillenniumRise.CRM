import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-brand-mark',
  templateUrl: './brand-mark.component.html',
  styleUrl: './brand-mark.component.css',
  standalone: false
})
export class BrandMarkComponent {
  @Input() compact = false;
  @Input() inverse = false;
}
