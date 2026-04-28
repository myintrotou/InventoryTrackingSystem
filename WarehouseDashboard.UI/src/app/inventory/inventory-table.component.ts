import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { InventoryService, Product } from '../services/inventory.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-inventory-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './inventory-table.component.html',
  styleUrls: ['./inventory-table.component.css']
})
export class InventoryTableComponent implements OnInit, AfterViewInit {
  viewMode: 'inventory' | 'orders' | 'settings' = 'inventory';
  displayedColumns: string[] = ['productID', 'productName', 'stockQuantity', 'reorderLevel', 'actions'];
  dataSource: MatTableDataSource<Product> = new MatTableDataSource();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inventoryService: InventoryService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {}

  logout() {
    this.authService.logout();
  }

  restock(product: Product) {
    const amount = 10;
    this.inventoryService.addStock(product.productID, amount).subscribe(() => {
      this.snackBar.open(`Added ${amount} units to ${product.productName}`, 'Success', { duration: 2000 });
      this.loadProducts();
    });
  }

  takeItem(product: Product) {
    const amount = 1; // Take 1 item at a time
    this.inventoryService.takeStock(product.productID, amount).subscribe({
      next: () => {
        this.snackBar.open(`Took 1 unit of ${product.productName}`, 'Updated', { duration: 2000 });
        this.loadProducts();
      },
      error: (err) => this.snackBar.open(err.error || 'Cannot take item', 'Error', { duration: 2000 })
    });
  }

  removeProduct(product: Product) {
    if (confirm(`Are you sure you want to delete ${product.productName}?`)) {
      this.inventoryService.deleteProduct(product.productID).subscribe(() => {
        this.snackBar.open(`${product.productName} removed from inventory`, 'Deleted', { duration: 2000 });
        this.loadProducts();
      });
    }
  }

  addNewItem() {
    const name = prompt("Enter Product Name:");
    if (!name) return;
    const qty = parseInt(prompt("Enter Initial Stock:") || "0");
    const reorder = parseInt(prompt("Enter Reorder Level:") || "10");

    this.inventoryService.addProduct({ productName: name, stockQuantity: qty, reorderLevel: reorder }).subscribe(() => {
      this.snackBar.open(`${name} added to inventory`, 'Success', { duration: 2000 });
      this.loadProducts();
    });
  }

  showComingSoon(feature: string) {
    this.snackBar.open(`${feature} feature is coming soon!`, 'Dismiss', {
      duration: 3000,
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  ngOnInit() {
    this.loadProducts();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getLowStockCount(): number {
    return this.dataSource.data.filter(p => p.stockQuantity <= p.reorderLevel).length;
  }

  getLowStockProducts() {
    return this.dataSource.data.filter(p => p.stockQuantity <= p.reorderLevel);
  }

  loadProducts() {
    this.inventoryService.getProducts().subscribe(data => {
      this.dataSource.data = data;
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  downloadReport() {
    this.inventoryService.downloadReport();
  }
}
