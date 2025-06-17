import { Routes } from '@angular/router';

import { Product } from './product/product';
import { About } from './about/about';
import { Products } from './products/products';
import { App } from './app';
import { Login } from './login/login';
import { Home } from './home/home';
import { Profile } from './profile/profile';
import { AuthGuard } from './authguard-guard';
import { ProductDetail } from './product-details/product-details';

export const routes: Routes = [
    {path:'login',component:Login},
    {path:'home/:un',component:Home,children:[
        {path:'products',component:Products,canActivate:[AuthGuard]},
        {path:'productDetails/:id',component:ProductDetail, canActivate:[AuthGuard]},
        {path:'about/:un',component:About},
        {path:'profile',component:Profile,canActivate:[AuthGuard]}
    ]},
    {path:'products',component:Products},
    {path:'about/:un',component:About}
];
