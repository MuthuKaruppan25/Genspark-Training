import { Routes } from '@angular/router';

import { Product } from './product/product';
import { About } from './about/about';
import { Products } from './products/products';
import { App } from './app';

export const routes: Routes = [
    {path:'home',component:App},
    {path:'products',component:Products},
    {path:'about',component:About}
];
