import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  isLogin = true;
  hide = true;
  user = { username: '', password: '' };

  constructor(private authService: AuthService, private snackBar: MatSnackBar) {}

  onSubmit() {
    if (this.isLogin) {
      this.authService.login(this.user).subscribe({
        next: () => {
          this.snackBar.open('Welcome back!', 'Close', { duration: 2000 });
          location.reload();
        },
        error: () => this.snackBar.open('Invalid credentials', 'Close', { duration: 2000 })
      });
    } else {
      this.authService.register(this.user).subscribe({
        next: () => {
          this.snackBar.open('Account created! Please log in.', 'Close', { duration: 2000 });
          this.isLogin = true;
        },
        error: () => this.snackBar.open('Username taken or error', 'Close', { duration: 2000 })
      });
    }
  }
}
