export class VideoUploadDto {

  constructor(public title:string,public description:string,public videoFile:File)
  {

  }

}

export interface VideoModel {
  id: string;
  title: string;
  description: string;
  uploadDate: string;
  blobUrl: string;
}