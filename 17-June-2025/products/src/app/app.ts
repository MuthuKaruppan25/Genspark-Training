import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected title = 'products';

  router = inject(ActivatedRoute);
  uname: string = '';
  constructor(private route: Router) {}
  ngOnInit(): void {
    console.log('init');
    this.uname = this.router.snapshot.params['un'] as string;
  }
  class: boolean = false;
  handleTap() {
    this.class = !this.class;
  }
  handleClick() {
    this.route.navigateByUrl('/about/' + this.uname);
  }
}
