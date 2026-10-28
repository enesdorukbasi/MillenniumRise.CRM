import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { FilesRoutingModule } from './files-routing.module';
import { FilesPageComponent } from './pages/files-page/files-page.component';

@NgModule({ declarations: [FilesPageComponent], imports: [SharedModule, FilesRoutingModule] })
export class FilesModule {}
