import { Injectable, signal, computed } from '@angular/core';
import { fromEvent } from 'rxjs';
import { startWith, map } from 'rxjs/operators';

export enum BreakpointSize {
  XS = 'xs',    // < 640px
  SM = 'sm',    // 640px - 768px  
  MD = 'md',    // 768px - 1024px
  LG = 'lg',    // 1024px - 1280px
  XL = 'xl'     // > 1280px
}

@Injectable({
  providedIn: 'root'
})
export class BreakpointService {
  // Why signals? Reactive, performant, and Angular's future!
  private currentWidthSignal = signal<number>(0);
  
  // Computed signals - automatically update when width changes
  currentBreakpoint = computed(() => {
    const width = this.currentWidthSignal();
    if (width < 640) return BreakpointSize.XS;
    if (width < 768) return BreakpointSize.SM; 
    if (width < 1024) return BreakpointSize.MD;
    if (width < 1280) return BreakpointSize.LG;
    return BreakpointSize.XL;
  });

  // Useful computed properties for layout decisions
  isMobile = computed(() => 
    [BreakpointSize.XS, BreakpointSize.SM].includes(this.currentBreakpoint())
  );
  
  isTablet = computed(() => 
    this.currentBreakpoint() === BreakpointSize.MD
  );
  
  isDesktop = computed(() => 
    [BreakpointSize.LG, BreakpointSize.XL].includes(this.currentBreakpoint())
  );

  constructor() {
    this.initializeBreakpointDetection();
  }

  private initializeBreakpointDetection(): void {
    // Initial width
    this.currentWidthSignal.set(window.innerWidth);
    
    // Listen to resize events
    fromEvent(window, 'resize')
      .pipe(
        startWith(null), // Emit immediately
        map(() => window.innerWidth)
      )
      .subscribe(width => {
        this.currentWidthSignal.set(width);
      });
  }
}
