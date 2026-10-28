import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BrandMarkComponent } from './components/brand-mark/brand-mark.component';
import { IconComponent } from './components/icon/icon.component';
import { ModalComponent } from './components/modal/modal.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { PaginationComponent } from './components/pagination/pagination.component';

@NgModule({
  declarations: [BrandMarkComponent, IconComponent, ModalComponent, PageHeaderComponent, PaginationComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  exports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule, BrandMarkComponent, IconComponent, ModalComponent, PageHeaderComponent, PaginationComponent]
})
export class SharedModule {}
