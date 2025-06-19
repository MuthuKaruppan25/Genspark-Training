import {
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import {
  BehaviorSubject,
  combineLatest,
  debounce,
  debounceTime,
  distinct,
  distinctUntilChanged,
  fromEvent,
  map,
  Observable,
  startWith,
} from 'rxjs';
import {
  selectAllUsers,
  selectUserError,
  selectUserLoading,
} from '../rxjs/user.selector';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { User } from '../Models/user';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css',
})
export class UserList implements OnInit {
  users$: Observable<User[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;

  searchTerm$ = new BehaviorSubject<string>('');
  selectedRole$ = new BehaviorSubject<string>(''); // NEW: for role filter

  filteredUsers$!: Observable<User[]>;

  @ViewChild('searchInput', { static: true }) searchInput!: ElementRef;
  @ViewChild('roleSelect', { static: true }) roleSelect!: ElementRef; // NEW: for role dropdown

  constructor(private store: Store) {
    this.users$ = this.store.select(selectAllUsers);
    this.loading$ = this.store.select(selectUserLoading);
    this.error$ = this.store.select(selectUserError);
  }

  ngOnInit(): void {
    
    fromEvent(this.searchInput.nativeElement, 'input')
      .pipe(
        map((event: any) => event.target.value.trim().toLowerCase()),
        debounceTime(500),
        distinctUntilChanged()
      )
      .subscribe(this.searchTerm$);

   
    fromEvent(this.roleSelect.nativeElement, 'change')
      .pipe(
        map((event: any) => event.target.value.trim().toLowerCase()),
        distinctUntilChanged()
      )
      .subscribe(this.selectedRole$);


    this.filteredUsers$ = combineLatest([
      this.users$,
      this.searchTerm$.pipe(startWith('')),
      this.selectedRole$.pipe(startWith('')),
    ]).pipe(
      map(([users, term, role]) =>
        users.filter((user) => {
          const matchesSearch =
            user.username.toLowerCase().includes(term) ||
            user.role.toLowerCase().includes(term);
          const matchesRole = !role || user.role.toLowerCase() === role;
          return matchesSearch && matchesRole;
        })
      )
    );
  }
}
