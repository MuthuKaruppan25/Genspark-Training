import { Component } from '@angular/core';

import { First } from "./first/first";
import { ProductList } from "./product-list/product-list";

@Component({
  selector: 'app-root',
  imports: [First, ProductList],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'firstApp';
}
