import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Videoupload } from "./videoupload/videoupload";
import { Showvideos } from "./showvideos/showvideos";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Videoupload, Showvideos,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'StreamingApp';
   showVideos = true;


  onEmitted(){
        this.showVideos = false;
    // re-render showvideos after short delay
    setTimeout(() => {
      this.showVideos = true;
    }, 500);
  }
}
