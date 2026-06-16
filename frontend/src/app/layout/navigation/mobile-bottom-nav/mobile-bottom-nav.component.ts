import { Component, ChangeDetectionStrategy } from '@angular/core';

import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-mobile-bottom-nav',
  standalone: true,
  imports: [RouterModule, LucideAngularModule],
  templateUrl: './mobile-bottom-nav.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./mobile-bottom-nav.component.scss']
})
export class MobileBottomNavComponent {}
