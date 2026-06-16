import { Component, Input, SecurityContext, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { marked } from 'marked';
import { SafeHtml, DomSanitizer } from '@angular/platform-browser';

import { MarkdownModule } from 'ngx-markdown';
import { CopyButtonComponent } from '../copy-button/copy-button.component';

@Component({
  selector: 'app-markdown-preview',
  imports: [MarkdownModule],
  templateUrl: './markdown-preview.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './markdown-preview.component.scss',
})
export class MarkdownPreviewComponent {
  @Input() content: string = '';
  readonly clipboardButton = CopyButtonComponent;
  renderedContent: SafeHtml = '';

  constructor(private sanitizer: DomSanitizer) {
    // Configure marked options
    marked.setOptions({
      breaks: true, // Convert \n to <br>
      gfm: true, // GitHub Flavored Markdown
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['content']) {
      this.renderMarkdown();
    }
  }

  private renderMarkdown(): void {
    if (!this.content) {
      this.renderedContent = '';
      return;
    }

    const html = marked.parse(this.content) as string;
    this.renderedContent = this.sanitizer.sanitize(SecurityContext.HTML, html) || '';
  }
}
