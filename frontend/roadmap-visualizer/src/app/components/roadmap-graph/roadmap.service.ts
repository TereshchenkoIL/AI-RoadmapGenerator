import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class RoadmapService {
  private apiUrl = 'http://localhost:5208/api/v1/roadmap/generate';

  constructor(private http: HttpClient) {}

  generateRoadmap(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }
}