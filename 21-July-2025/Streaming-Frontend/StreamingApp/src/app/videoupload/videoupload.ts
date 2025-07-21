import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { VideoService } from '../Services/video.service';
import { VideoUploadDto } from '../Models/video.model';
import { ViewChild, ElementRef } from '@angular/core';
@Component({
  selector: 'app-videoupload',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './videoupload.html',
  styleUrls: ['./videoupload.css'],
})
export class Videoupload {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  uploadForm: FormGroup;
  selectedFile?: File;
  uploadSuccess = false;
  uploadError = '';
  @Output()  uploadEmitEvent : EventEmitter<void> = new EventEmitter<void>();

  isUploading = false;

  isInvalid(controlName: string): boolean {
    const control = this.uploadForm.get(controlName);
    return !!(control && control.invalid && control.touched);
  }

  constructor(private fb: FormBuilder, private videoservice: VideoService) {
    this.uploadForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      videoFile: [null, [Validators.required]],
    });
  }

  onFileChange(event: any) {
    const file: File = event.target.files[0];
    const control = this.uploadForm.get('videoFile');

    if (file && file.type.startsWith('video/')) {
      this.selectedFile = file;
      control?.setValue(file);
      control?.setErrors(null);
      control?.markAsTouched(); // ✅ Important
    } else {
      control?.setValue(null);
      control?.setErrors({ invalidType: true });
      control?.markAsTouched(); // ✅ Important to show error
      this.selectedFile = undefined;
    }
  }


  upload() {
    if (this.uploadForm.invalid || !this.selectedFile) return;

    const formData = new FormData();
    formData.append('title', this.uploadForm.value.title);
    formData.append('description', this.uploadForm.value.description);
    formData.append('videoFile', this.selectedFile);

    this.isUploading = true;
    this.uploadSuccess = false;
    this.uploadError = '';
    this.videoservice.uploadVideo(formData).subscribe({
      next: (data: any) => {
        this.uploadSuccess = true;
        this.uploadError = '';
        this.uploadForm.reset();
        this.selectedFile = undefined;
        this.fileInput.nativeElement.value = '';
        this.uploadEmitEvent.emit(); // Emit event on successful upload
      },
      error: (err) => {
        this.uploadError = err?.error?.message || 'Upload failed';
      },
      complete: () => {
        this.isUploading = false;
      },
    });
  }
}
