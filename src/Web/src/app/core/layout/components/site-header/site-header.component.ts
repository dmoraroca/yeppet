import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-site-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './site-header.component.html',
  styleUrl: './site-header.component.scss'
})
export class SiteHeaderComponent {
  @ViewChild('helpDropdown') private readonly helpDropdown?: ElementRef<HTMLDetailsElement>;

  protected closeHelpDropdown(): void {
    if (this.helpDropdown?.nativeElement.open) {
      this.helpDropdown.nativeElement.open = false;
    }
  }

  @HostListener('document:click', ['$event'])
  protected onDocumentClick(event: MouseEvent): void {
    const dropdown = this.helpDropdown?.nativeElement;

    if (!dropdown || !dropdown.open) {
      return;
    }

    const target = event.target as Node | null;

    if (target && !dropdown.contains(target)) {
      dropdown.open = false;
    }
  }

  @HostListener('document:keydown.escape')
  protected onEscape(): void {
    this.closeHelpDropdown();
  }
}
