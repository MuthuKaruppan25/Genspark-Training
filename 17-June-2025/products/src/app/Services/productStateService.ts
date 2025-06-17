
import { Injectable } from '@angular/core';
import { ProductModel } from '../Models/productModel';

@Injectable({ providedIn: 'root' })
export class ProductStateService {
  products: ProductModel[] = [];
  searchText: string = '';
  skip: number = 0;
  total: number = 0;
  scrollPosition: number = 0;
}
