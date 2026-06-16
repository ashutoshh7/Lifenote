import { Component, inject, ChangeDetectionStrategy } from '@angular/core';

import { RouterModule } from '@angular/router';
import { LayoutService } from '../../../core/services/layout.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-desktop-sidebar',
  standalone: true,
  imports: [RouterModule, LucideAngularModule],
  templateUrl: './desktop-sidebar.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./desktop-sidebar.component.scss']
})
export class DesktopSidebarComponent {
  layoutService = inject(LayoutService);
}

