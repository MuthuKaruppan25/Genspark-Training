import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Adduser } from './adduser/adduser';
import { Dashboard } from './dashboard/dashboard';

export const routes: Routes = [
    {path:'home',component:Home,children:[
        {path:'adduser',component:Adduser},
        {path:'dashboard',component:Dashboard}
    ]}
];
