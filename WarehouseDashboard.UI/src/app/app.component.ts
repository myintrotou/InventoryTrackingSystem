import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { InventoryTableComponent } from './inventory/inventory-table.component';
import { AuthComponent } from './auth/auth.component';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, InventoryTableComponent, AuthComponent],
  template: `
    <app-auth *ngIf="!isAuthenticated"></app-auth>
    <app-inventory-table *ngIf="isAuthenticated"></app-inventory-table>
  `,
  styles: []
})
export class AppComponent implements OnInit {
  isAuthenticated = false;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
  }
}
