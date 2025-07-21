import { Component, OnInit } from '@angular/core';
import { VideoModel } from '../Models/video.model';
import { VideoService } from '../Services/video.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-showvideos',
  imports: [CommonModule, FormsModule],
  templateUrl: './showvideos.html',
  styleUrl: './showvideos.css',
})
export class Showvideos implements OnInit {
  videos!: VideoModel[];
  searchQuery = '';
  private searchSubject = new Subject<string>();

  constructor(private videoService: VideoService) {}

  ngOnInit(): void {
    // Debounce search input to reduce rapid calls
    this.searchSubject.pipe(debounceTime(300)).subscribe((query:string) => {
      this.fetchVideos(query);
    });

    // Initial load
    this.fetchVideos();
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchQuery);
  }

  fetchVideos(query: string = ''): void {
    console.log('Fetching videos with query:', query);
    this.videoService.getAllVideos(query).subscribe({
      next: (data) => (this.videos = data),
    });
  }

  getStreamUrl(id: string): string {
    return this.videoService.getStreamUrl(id);
  }
}
