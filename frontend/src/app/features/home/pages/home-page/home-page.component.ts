import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Welcome to Momentum!</h1>
      <p class="mt-2 text-lg text-gray-600 dark:text-gray-400">
        Select a feature from the sidebar to get started.
      </p>
    </div>
  `,
  styles: []
})
export class HomePageComponent {

}
