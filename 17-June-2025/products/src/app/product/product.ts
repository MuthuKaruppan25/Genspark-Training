import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { ProductModel } from '../Models/productModel';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-product',
  imports: [],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class Product {
  @Input() product: ProductModel | undefined = undefined;
  @Input() searchTerm: string = '';

  constructor(private router: Router, private route: ActivatedRoute) {}

  highlightMatch(text: string): string {
    if (!this.searchTerm.trim()) return text;
    const escaped = this.searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const regex = new RegExp(`(${escaped})`, 'gi');
    return text.replace(regex, '<mark>$1</mark>');
  }
  handleBuy(id: number) {
    this.router.navigate(['../productDetails', id], { relativeTo: this.route });
  }

}
