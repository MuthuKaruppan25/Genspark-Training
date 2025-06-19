import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { map, Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { loadUsers, loadUsersSuccess } from '../rxjs/user.actions';
