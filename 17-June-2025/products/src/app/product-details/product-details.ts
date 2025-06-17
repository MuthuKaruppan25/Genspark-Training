import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../Services/productService';
import { ActivatedRoute } from '@angular/router';
import { ProductDetails } from '../Models/productDetails';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css'
})
export class ProductDetail implements OnInit {
  product : ProductDetails | undefined;
  productId:number = 0;
   private route = inject(ActivatedRoute);
  constructor(private productService: ProductService)
  {

  }
  ngOnInit(): void {
    this.productId = +this.route.snapshot.params['id']; 
    this.productService.getProductDetails(this.productId).subscribe({
      next:(data:any)=>{
        this.product = ProductDetails.FromForm(data);
        console.log("Product Details "+this.product);
      }
    })
  }

}
