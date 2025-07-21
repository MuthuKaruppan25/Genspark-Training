import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { VideoModel, VideoUploadDto } from '../Models/video.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VideoService {
  private baseUrl = 'http://localhost:5239/api/Videos';

  private http = inject(HttpClient);

  uploadVideo(data: FormData): Observable<VideoModel> {
    return this.http.post<VideoModel>(`${this.baseUrl}/upload`, data);
  }

getAllVideos(query: string = '') {
  const url = `${this.baseUrl}?query=${query}`;
    
console.log('Fetching videos from URL:', url);
  return this.http.get<VideoModel[]>(url);
}
  getStreamUrl(id: string): string {
    return `${this.baseUrl}/stream/${id}`;
  }
}
