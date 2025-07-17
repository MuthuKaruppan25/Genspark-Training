import { Routes } from '@angular/router';
import { PaymentComponent } from './payment-component/payment-component';

export const routes: Routes = [
    {
        path:'payment',
        component:PaymentComponent
    },
     { path: '', redirectTo: 'payment', pathMatch: 'full' },
];
