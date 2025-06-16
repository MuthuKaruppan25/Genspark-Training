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
  showButton:boolean = false;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {

    this.searchSubject
      .pipe(
        debounceTime(800),
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
  handleSearchProducts(){
    this.skip=0;
    this.searchSubject.next(this.searchText);
  }
  @HostListener('window:scroll', [])
  onScroll(): void {
    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.body.offsetHeight - 10;
    this.showButton = window.scrollY > 200;
    console.log("Length"+this.products.length);
    if (scrollPosition >= threshold && this.products?.length < this.total) {
      console.log(scrollPosition);
      console.log(threshold);

      this.loadMore();
    }
  }
  scrolToTop():void{
    window.scrollTo({top:0,behavior:'smooth'});
  }
  loadMore() {
    this.loading = true;
    this.skip += this.limit;
    this.productService
      .getProductsSearchResult(this.searchText, this.limit, this.skip)
      .subscribe({
        next: (data: any) => {
          const current = this.products();
          this.products.set([...current,...data.products]);
          this.loading = false;
        },
      });
  }
}
