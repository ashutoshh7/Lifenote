import { Component, ChangeDetectionStrategy } from '@angular/core';


@Component({
  selector: 'app-copy-button',
  standalone: true,
  imports: [],
  template: `
    <button
      class="btn-copy"
      aria-label="Copy to clipboard"
      (click)="onCopyClick()"
    >
      @if (!isCopied) {
        <i class="fa-regular fa-copy icon fade-in"></i>
      }
      @if (isCopied) {
        <i class="fa-solid fa-check icon fade-in"></i>
      }
    </button>
  `,
  changeDetection: ChangeDetectionStrategy.Eager,
  styles: [`
    .btn-copy {
      position: absolute;
      top: 5px;
      right: 5px;
      z-index: 10;
      padding: 4px 8px;
      font-size: 12px;
      color: #fff;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.2s;
    }
    .btn-copy:hover {
      background-color: rgba(255, 255, 255, 0.2);
    }
    .icon {
      font-size: 14px;
      display: inline-block;
    }
    
    /* Fade-in animation for icon changes */
    .fade-in {
      animation: fadeIn 0.3s ease-in;
    }
    
    @keyframes fadeIn {
      from {
        opacity: 0;
        transform: scale(0.8);
      }
      to {
        opacity: 1;
        transform: scale(1);
      }
    }
  `]
})
export class CopyButtonComponent {
  // ngx-markdown provides this as a signal
  copied: () => boolean = () => false;
  isCopied: boolean = false;

  onCopyClick(): void {
    // ngx-markdown handles the actual copy
    this.isCopied = true;
    setTimeout(() => {
      this.isCopied = false;
    }, 2500);
  }
}
