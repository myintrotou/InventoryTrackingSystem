import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  productID: number;
  productName: string;
  stockQuantity: number;
  reorderLevel: number;
  supplier?: { supplierName: string };
}

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private apiUrl = `${environment.apiUrl}/inventory`;

  constructor(private http: HttpClient) { }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/products`);
  }

  addStock(productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/add-stock/${productId}`, quantity);
  }

  takeStock(productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/take-stock/${productId}`, quantity);
  }

  addProduct(product: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/add-product`, product);
  }

  deleteProduct(productId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-product/${productId}`);
  }

  downloadReport() {
    window.open(`${this.apiUrl}/report`, '_blank');
  }
}
