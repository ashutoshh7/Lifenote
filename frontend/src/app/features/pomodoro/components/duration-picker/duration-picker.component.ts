import { Component, input, output, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-duration-picker',
  standalone: true,
  templateUrl: './duration-picker.component.html',
  styleUrls: ['./duration-picker.component.scss'],
  changeDetection: ChangeDetectionStrategy.Eager
})
export class DurationPickerComponent implements OnInit {
  initialHours = input<number>(0);
  initialMinutes = input<number>(25);
  initialSeconds = input<number>(0);

  save = output<{ hours: number; minutes: number; seconds: number }>();
  cancel = output<void>();

  editHours = 0;
  editMinutes = 25;
  editSeconds = 0;

  hoursList = Array.from({ length: 24 }, (_, i) => i);
  minutesList = Array.from({ length: 60 }, (_, i) => i);
  secondsList = Array.from({ length: 60 }, (_, i) => i);

  ngOnInit() {
    this.editHours = this.initialHours();
    this.editMinutes = this.initialMinutes();
    this.editSeconds = this.initialSeconds();
  }

  limitValue(event: Event, min: number, max: number, field: 'hours' | 'minutes' | 'seconds'): void {
    const inputEl = event.target as HTMLInputElement;
    let val = parseInt(inputEl.value, 10);
    if (isNaN(val)) val = 0;
    if (val < min) val = min;
    if (val > max) val = max;

    if (field === 'hours') this.editHours = val;
    else if (field === 'minutes') this.editMinutes = val;
    else if (field === 'seconds') this.editSeconds = val;

    inputEl.value = val.toString();
  }

  onDrumScroll(event: Event, type: 'hours' | 'minutes' | 'seconds') {
    const el = event.target as HTMLElement;
    const itemHeight = 40;
    const index = Math.round(el.scrollTop / itemHeight);

    if (type === 'hours') {
      this.editHours = Math.min(Math.max(0, index), 23);
    } else if (type === 'minutes') {
      this.editMinutes = Math.min(Math.max(0, index), 59);
    } else if (type === 'seconds') {
      this.editSeconds = Math.min(Math.max(0, index), 59);
    }
  }

  onDrumWheel(event: WheelEvent, type: 'hours' | 'minutes' | 'seconds') {
    event.preventDefault();
    const el = event.currentTarget as HTMLElement;
    const itemHeight = 40;

    const direction = Math.sign(event.deltaY);
    const currentSnap = Math.round(el.scrollTop / itemHeight);
    const nextSnap = currentSnap + direction;

    el.scrollTo({
      top: nextSnap * itemHeight,
      behavior: 'smooth'
    });
  }

  saveDuration() {
    this.save.emit({
      hours: this.editHours,
      minutes: this.editMinutes,
      seconds: this.editSeconds
    });
  }

  cancelDuration() {
    this.cancel.emit();
  }
}
