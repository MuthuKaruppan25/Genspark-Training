import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-snackbar',
  imports: [],
  templateUrl: './snackbar.html',
  styleUrl: './snackbar.css'
})
export class Snackbar {
  @Input() message : string = "";
  @Input() color : string = "";
}
