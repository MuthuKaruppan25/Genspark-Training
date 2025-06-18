import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [RouterLink,RouterOutlet],
  standalone:true,
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected title = 'products';

  class: boolean = false;
  handleTap() {
    this.class = !this.class;
  }
}
