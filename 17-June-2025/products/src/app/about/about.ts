import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-about',
  imports: [],
  templateUrl: './about.html',
  styleUrl: './about.css'
})
export class About {
    router = inject(ActivatedRoute)
  uname: string = '';

  ngOnInit(): void {
    console.log('init');
    this.uname = this.router.snapshot.params['un'] as string;
  }
}
