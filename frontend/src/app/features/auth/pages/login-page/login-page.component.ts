import { Component, inject, ChangeDetectionStrategy } from '@angular/core';

import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators, ValidatorFn, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';

export function strongPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;

    const hasUpperCase = /[A-Z]+/.test(value);
    const hasLowerCase = /[a-z]+/.test(value);
    const hasNumeric = /[0-9]+/.test(value);
    const hasSpecial = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(value);
    const isValidLength = value.length >= 8;

    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial && isValidLength;

    if (!passwordValid) {
      return { 
        strongPassword: {
          hasUpperCase,
          hasLowerCase,
          hasNumeric,
          hasSpecial,
          isValidLength
        }
      };
    }
    return null;
  };
}

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  isSignupMode = false;
  isResetMode = false;
  isLoading = false;
  showPassword = false;
  isResetEmailSent = false;

  // Focus state for field animation
  emailFocused = false;
  pwFocused = false;
  signupEmailFocused = false;
  usernameFocused = false;
  firstNameFocused = false;
  lastNameFocused = false;
  signupPwFocused = false;
  confirmPwFocused = false;
  resetEmailFocused = false;

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  signupForm = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, strongPasswordValidator()]],
    confirmPassword: ['', Validators.required]
  }, { validators: this.passwordMatchValidator });

  resetForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  passwordMatchValidator(group: AbstractControl) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  errorMessage = '';

  toggleMode() {
    this.isSignupMode = !this.isSignupMode;
    this.isResetMode = false;
    this.isResetEmailSent = false;
    this.errorMessage = '';
    this.showPassword = false;
    this.loginForm.reset();
    this.signupForm.reset();
    this.resetForm.reset();
  }

  switchToResetMode() {
    this.isResetMode = true;
    this.isSignupMode = false;
    this.isResetEmailSent = false;
    this.errorMessage = '';
    this.resetForm.reset();
  }

  sendResetLink() {
    if (this.resetForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      const email = this.resetForm.value.email;
      this.authService.sendPasswordResetEmail(email!).subscribe({
        next: () => {
          this.isLoading = false;
          this.isResetEmailSent = true;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = this.getErrorMessage(err?.code ?? '');
        }
      });
    }
  }

  login() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      const { email, password } = this.loginForm.value;
      
      this.authService.login(email!, password!).subscribe({
        next: () => {
          // Keep isLoading true while redirecting
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = this.getErrorMessage(err?.code ?? '');
        }
      });
    }
  }

  async signup() {
    if (this.signupForm.valid) {
      const { email, password, confirmPassword, username, firstName, lastName } = this.signupForm.value;
      this.errorMessage = '';

      if (password !== confirmPassword) {
        this.errorMessage = 'Passwords do not match';
        return;
      }

      this.isLoading = true;
      this.authService.signUp(email!, password!, username!, firstName!, lastName!).subscribe({
        next: (user) => {
          // Keep isLoading true while redirecting
          console.log('User created:', user);
        },
        error: (err) => {
          this.isLoading = false;
          if (err.message === 'USERNAME_TAKEN') {
            this.errorMessage = 'Username is already taken';
          } else {
            this.errorMessage = this.getErrorMessage(err?.code ?? '');
          }
        }
      });
    }
  }

  private getErrorMessage(errorCode: string): string {
    switch (errorCode) {
      case 'auth/email-already-in-use':
        return 'This email is already registered';
      case 'auth/invalid-email':
        return 'Invalid email address';
      case 'auth/weak-password':
        return 'Password should be at least 6 characters';
      case 'auth/user-not-found':
        return 'No user found with this email address';
      default:
        return 'An error occurred. Please try again';
    }
  }
}
