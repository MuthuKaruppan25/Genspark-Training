import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Adduser } from './adduser/adduser';

import { UserList } from './user-list/user-list';

export const routes: Routes = [
    {path:'home',component:Home,children:[
        {path:'adduser',component:Adduser},
        {path:'dashboard',component:UserList}
    ]}
];
