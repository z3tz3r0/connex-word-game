import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { FormInput } from '../../shared/form-input/form-input';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormInput, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
})
export class Register {
  registerForm: FormGroup;
  registerError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      isVIP: [false],
    });
  }

  get username() {
    return this.registerForm.get('username') as FormControl;
  }
  get password() {
    return this.registerForm.get('password') as FormControl;
  }
  get isVIP() {
    return this.registerForm.get('isVIP') as FormControl;
  }

  onSubmit() {
    this.registerError = null;
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          // console.log('Registration successful!', response);
          alert('Registration Successful! Please log in.'); // แจ้งเตือนผู้ใช้
          this.router.navigate(['/login']); // พาไปหน้า Login
        },
        error: (err) => {
          // console.error('Registration failed', err);
          this.registerError =
            err.error?.message || 'An unknown error occurred.';
        },
      });
    }
  }
}
