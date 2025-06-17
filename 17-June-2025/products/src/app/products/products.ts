import { Component, signal, OnInit, HostListener } from '@angular/core';
import { ProductModel } from '../Models/productModel';
import {
  debounceTime,
  distinctUntilChanged,
  Subject,
  switchMap,
  tap,
} from 'rxjs';
import { ProductService } from '../Services/productService';
import { FormsModule } from '@angular/forms';
import { Product } from '../product/product';
import { ProductStateService } from '../Services/productStateService';

@Component({
  selector: 'app-products',
  imports: [FormsModule, Product],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  products = signal<ProductModel[]>([]);
  loading: boolean = false;
  searchText: string = '';
  searchSubject = new Subject<string>();
  limit: number = 10;
  skip: number = 0;
  total: number = 0;
  showButton: boolean = false;

  constructor(
    private productService: ProductService,
    private productState: ProductStateService
  ) {}

  ngOnInit(): void {
    if (this.productState.products.length) {
      this.products.set(this.productState.products);
      this.searchText = this.productState.searchText;
      this.skip = this.productState.skip;
      this.total = this.productState.total;

      setTimeout(
        () => window.scrollTo({ top: this.productState.scrollPosition }),
        0
      );
      return;
    }

    this.searchSubject
      .pipe(
        debounceTime(1000),
        distinctUntilChanged(),
        tap(() => (this.loading = true)),
        switchMap((query) =>
          this.productService.getProductsSearchResult(
            query,
            this.limit,
            this.skip
          )
        ),
        tap(() => (this.loading = false))
      )
      .subscribe({
        next: (data: any) => {
          this.products.set(data?.products as ProductModel[]);
          this.total = data.total;
          console.log(this.total);
        },
      });
  }
  handleSearchProducts() {
    this.skip = 0; // Reset skip
    this.productState.products = []; // Reset cache
    this.productState.searchText = this.searchText;
    this.searchSubject.next(this.searchText);
  }
  @HostListener('window:scroll', [])
  onScroll(): void {
    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.body.offsetHeight - 100;
    this.showButton = window.scrollY > 200;
    console.log('Length' + this.products.length);
    if (scrollPosition >= threshold && this.products?.length < this.total) {
      this.loadMore();
    }
  }
  scrolToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  loadMore() {
    this.loading = true;
    this.skip += this.limit;
    this.productService
      .getProductsSearchResult(this.searchText, this.limit, this.skip)
      .subscribe({
        next: (data: any) => {
          const current = this.products();
          const updated = [...current, ...data.products];
          this.products.set(updated);


          this.productState.products = updated;
          this.productState.skip = this.skip;
          this.productState.total = this.total;
          this.loading = false;
        },
      });
  }
}
