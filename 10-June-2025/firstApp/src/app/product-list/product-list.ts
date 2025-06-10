import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';


export interface Product {
  name: string;
  img: string;
  price: number;
}

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.html',
  styleUrls: ['./product-list.css']
})
export class ProductList {
  cartCount : number= 0;

  
  products: Product[] = [
    { name: 'Mobile', img: 'assets/mobike.jpeg', price: 20 },
    { name: 'Headset', img: 'assets/product.jpeg', price: 40 },
    { name: 'Laptop', img: 'assets/download.jpeg', price: 60 }
  ];

  addToCart() {
    this.cartCount++;
  }
}