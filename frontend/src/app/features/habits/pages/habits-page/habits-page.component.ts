import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Plus } from 'lucide-angular';
import { LayoutService } from '../../../../core/services/layout.service';
import { HabitListComponent } from "../../habit-list/habit-list.component";
import { PageHeaderComponent } from '../../../../shared';

@Component({
  selector: 'app-habits-page',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, HabitListComponent, PageHeaderComponent],
  templateUrl: './habits-page.component.html',
  styleUrls: ['./habits-page.component.scss']
})
export class HabitsPageComponent implements OnInit {
  private layoutService = inject(LayoutService);
  PlusIcon = Plus;

  ngOnInit(): void {
  }
}