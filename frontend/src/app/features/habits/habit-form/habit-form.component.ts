import {
  Component,
  Inject,
  OnInit,
  OnDestroy,
  signal,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  MatDialogModule,
  MAT_DIALOG_DATA,
  MatDialogRef
} from '@angular/material/dialog';
import { LucideAngularModule, Check, Plus, Minus } from 'lucide-angular';
import { Habit, CreateHabitDto } from '../models/habit.model';
import { HabitService } from '../services/habit.service';
import { Subject, takeUntil } from 'rxjs';

export interface HabitFormDialogData {
  habit?: Habit;
}

interface ColorOption { value: string; label: string; }
interface DayOption   { value: string; label: string; selected: boolean; }

/**
 * Returns true if the string is exactly one emoji grapheme cluster.
 * Uses Intl.Segmenter when available (Chrome 87+, Safari 15.4+, FF 116+);
 * falls back to a broad unicode emoji regex.
 */
function isSingleEmoji(str: string): boolean {
  if (!str) return false;
  if (typeof Intl !== 'undefined' && (Intl as any).Segmenter) {
    const segments = [...new (Intl as any).Segmenter().segment(str)];
    return segments.length === 1;
  }
  // Fallback: strip everything that is NOT an emoji / symbol codepoint
  const emojiRx = /^(\p{Emoji_Presentation}|\p{Extended_Pictographic})(\uFE0F)?(\u20E3|[\uE0020-\uE007E]+\uE007F)?(\uD83C[\uDFFB-\uDFFF])?(\u200D(\p{Emoji_Presentation}|\p{Extended_Pictographic})(\uFE0F)?(\uD83C[\uDFFB-\uDFFF])?)*$/u;
  return emojiRx.test(str.trim());
}

/**
 * Extract the first grapheme cluster from an arbitrary string.
 * Used to cap the emoji input at 1 character.
 */
function firstGrapheme(str: string): string {
  if (!str) return '';
  if (typeof Intl !== 'undefined' && (Intl as any).Segmenter) {
    const [first] = new (Intl as any).Segmenter().segment(str);
    return first?.segment ?? str[0];
  }
  // Fallback: take up to 2 code-units (handles basic surrogate pairs)
  return [...str][0] ?? '';
}

@Component({
  selector: 'app-habit-form-dialog',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, LucideAngularModule],
  templateUrl: './habit-form.component.html',
  styleUrl: './habit-form.component.scss'
})
export class HabitFormComponent implements OnInit, OnDestroy {
  /* ── lucide icons ── */
  CheckIcon = Check;
  PlusIcon  = Plus;
  MinusIcon = Minus;

  /* ── state ── */
  habitForm!: FormGroup;
  submitting   = signal(false);
  errorMessage = signal<string | null>(null);
  isEditMode   = false;

  /* ── live preview signals ── */
  previewName  = signal('My new habit');
  previewIcon  = signal('\uD83D\uDCAA'); // 💪
  previewColor = signal('#4CAF50');

  /* ── color options ── */
  readonly availableColors: ColorOption[] = [
    { value: '#4CAF50', label: 'Emerald' },
    { value: '#2196F3', label: 'Blue'    },
    { value: '#FF9800', label: 'Amber'   },
    { value: '#F44336', label: 'Red'     },
    { value: '#9C27B0', label: 'Purple'  },
    { value: '#E91E63', label: 'Pink'    },
    { value: '#00BCD4', label: 'Cyan'    },
    { value: '#607D8B', label: 'Slate'   }
  ];

  daysOfWeek: DayOption[] = [
    { value: 'Monday',    label: 'Mon', selected: false },
    { value: 'Tuesday',   label: 'Tue', selected: false },
    { value: 'Wednesday', label: 'Wed', selected: false },
    { value: 'Thursday',  label: 'Thu', selected: false },
    { value: 'Friday',    label: 'Fri', selected: false },
    { value: 'Saturday',  label: 'Sat', selected: false },
    { value: 'Sunday',    label: 'Sun', selected: false }
  ];

  /* ── stepper hold-to-accelerate ── */
  private stepInterval: ReturnType<typeof setInterval>  | null = null;
  private stepTimeout:  ReturnType<typeof setTimeout>   | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private fb:           FormBuilder,
    private habitService: HabitService,
    public  dialogRef:    MatDialogRef<HabitFormComponent>,
    private cdr:          ChangeDetectorRef,
    @Inject(MAT_DIALOG_DATA) public data: HabitFormDialogData
  ) {
    this.isEditMode = !!data?.habit;
  }

  ngOnInit(): void {
    const h = this.data?.habit;
    this.habitForm = this.fb.group({
      name:          [h?.name          ?? '',        [Validators.required, Validators.maxLength(200)]],
      description:   [h?.description   ?? '',        [Validators.maxLength(500)]],
      frequencyType: [h?.frequencyType ?? 'Daily',   Validators.required],
      targetCount:   [h?.targetCount   ?? 1,         [Validators.required, Validators.min(1), Validators.max(10)]],
      iconName:      [h?.iconName      ?? '\uD83D\uDCAA', Validators.required],
      color:         [h?.color         ?? '#4CAF50', Validators.required]
    });

    // Populate edit-mode day selections
    if (this.isEditMode && h?.frequencyType === 'Custom' && h.frequencyValue) {
      try {
        const days: string[] = JSON.parse(h.frequencyValue);
        this.daysOfWeek.forEach(d => d.selected = days.includes(d.value));
      } catch { /* ignore */ }
    }

    // Sync preview signals live
    this.habitForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(v => {
      this.previewName.set(v.name?.trim() || 'My new habit');
      if (v.iconName) this.previewIcon.set(v.iconName);
      if (v.color)    this.previewColor.set(v.color);
      this.cdr.markForCheck();
    });

    // Init preview from existing values
    const fv = this.habitForm.value;
    this.previewName.set(fv.name?.trim() || 'My new habit');
    this.previewIcon.set(fv.iconName);
    this.previewColor.set(fv.color);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.clearStep();
  }

  /* ─── getters ─────────────────────────────────────── */
  get frequencyType(): string { return this.habitForm.get('frequencyType')?.value; }
  get isCustom():     boolean { return this.frequencyType === 'Custom'; }
  get targetCount():  number  { return this.habitForm.get('targetCount')?.value ?? 1; }

  /* ─── emoji input handler ─────────────────────────── */
  onEmojiInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const raw   = input.value;

    if (!raw) {
      // Allow empty so user can clear and retype
      this.habitForm.patchValue({ iconName: '' }, { emitEvent: false });
      this.cdr.markForCheck();
      return;
    }

    // Take only the first grapheme cluster (handles ZWJ sequences, skin tones, flags)
    const first = firstGrapheme(raw);

    // Accept it regardless — we don't block text, but we do trim to 1 grapheme.
    // This means if the user types a letter it shows, but the preview won't update
    // unless it's an emoji.  A stricter guard (isSingleEmoji) is commented below.
    input.value = first;
    this.habitForm.patchValue({ iconName: first }, { emitEvent: false });

    // Update preview immediately
    this.previewIcon.set(isSingleEmoji(first) ? first : this.previewIcon());
    this.cdr.markForCheck();
  }

  /* ─── color picker ────────────────────────────────── */
  selectColor(color: string): void { this.habitForm.patchValue({ color }); }

  /* ─── day toggler ─────────────────────────────────── */
  toggleDay(day: DayOption): void { day.selected = !day.selected; this.cdr.markForCheck(); }

  /* ─── frequency ───────────────────────────────────── */
  setFrequency(type: string): void { this.habitForm.patchValue({ frequencyType: type }); }

  /* ─── stepper ─────────────────────────────────────── */
  increment(): void {
    if (this.targetCount < 10)
      this.habitForm.patchValue({ targetCount: this.targetCount + 1 });
  }

  decrement(): void {
    if (this.targetCount > 1)
      this.habitForm.patchValue({ targetCount: this.targetCount - 1 });
  }

  startStep(direction: 'up' | 'down'): void {
    this.applyStep(direction);
    this.stepTimeout = setTimeout(() => {
      this.stepInterval = setInterval(() => this.applyStep(direction), 80);
    }, 400);
  }

  endStep(): void { this.clearStep(); }

  private applyStep(dir: 'up' | 'down'): void {
    dir === 'up' ? this.increment() : this.decrement();
    this.cdr.markForCheck();
  }

  private clearStep(): void {
    if (this.stepTimeout)  { clearTimeout(this.stepTimeout);   this.stepTimeout  = null; }
    if (this.stepInterval) { clearInterval(this.stepInterval); this.stepInterval = null; }
  }

  /* ─── submit ──────────────────────────────────────── */
  onSubmit(): void {
    this.habitForm.markAllAsTouched();
    if (this.habitForm.invalid || this.submitting()) return;

    if (this.isCustom) {
      const selected = this.daysOfWeek.filter(d => d.selected);
      if (!selected.length) {
        this.errorMessage.set('Select at least one day for a custom schedule.');
        return;
      }
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    const fv = this.habitForm.value;
    const dto: CreateHabitDto = {
      name:           fv.name.trim(),
      description:    fv.description?.trim() || undefined,
      color:          fv.color,
      iconName:       fv.iconName,
      frequencyType:  fv.frequencyType,
      frequencyValue: this.isCustom
        ? JSON.stringify(this.daysOfWeek.filter(d => d.selected).map(d => d.value))
        : undefined,
      targetCount:    fv.targetCount
    };

    const req = this.isEditMode && this.data.habit
      ? this.habitService.updateHabit(this.data.habit.id, dto)
      : this.habitService.createHabit(dto);

    req.subscribe({
      next:  (habit) => { this.dialogRef.close({ success: true, habit }); },
      error: (err)   => {
        console.error(err);
        this.submitting.set(false);
        this.errorMessage.set('Something went wrong. Please try again.');
        this.cdr.markForCheck();
      }
    });
  }

  onCancel(): void { this.dialogRef.close({ success: false }); }
}
