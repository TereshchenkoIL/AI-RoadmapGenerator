import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgxGraphModule } from '@swimlane/ngx-graph';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { RoadmapService } from './roadmap.service';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { SettingsDialogComponent } from '../settings-dialog/settings-dialog.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-roadmap-graph',
  imports: [
    NgxGraphModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    CommonModule,
    MatDialogModule,
    MatProgressSpinnerModule],
  templateUrl: './roadmap-graph.component.html',
  styleUrl: './roadmap-graph.component.scss'
})
export class RoadmapGraphComponent implements OnInit {
  nodes: any[] = [];
  links: any[] = [];
  userJson: string = '';
  models: string[] = ['gpt-3.5-turbo', 'gpt-4o', 'gpt-4o-mini', 'o3-mini'];
  selectedModel: string = this.models[0];
  isLoading = false;

  constructor(private dialog: MatDialog, private roadmapService: RoadmapService) {}

  ngOnInit(): void {
    // Поки що просто мокнуті дані
    this.nodes = [
      { id: 'step1', label: 'Крок 1: Основи .NET' },
      { id: 'topic1', label: '.NET Framework' },
      { id: 'subtopic1', label: '.NET Core' }
    ];

    this.links = [
      { source: 'step1', target: 'topic1' },
      { source: 'topic1', target: 'subtopic1' }
    ];
  }


  openSettings() {
    const dialogRef = this.dialog.open(SettingsDialogComponent, {
      width: '700px',
      maxHeight: '90vh', // обмеження по висоті
      data: {
        userJson: this.userJson,
        model: this.selectedModel,
        models: this.models
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userJson = result.userJson;
        this.selectedModel = result.model;
        this.sendRequest(result.parsed);
      }
    });
  }

  onSubmit() {
    try {
      const parsed = JSON.parse(this.userJson);
  
      const payload = {
        ...parsed,
        model: this.selectedModel
      };
  
      this.roadmapService.generateRoadmap(payload).subscribe({
        next: (response: any) => {
          try {
            const roadmap = JSON.parse(response.roadmap); // ⬅️ Ось тут перший парсинг рядка в об'єкт
            this.buildGraphFromRoadmap(roadmap);          // ⬅️ Далі передаємо у функцію побудови графа
          } catch (e: any) {
            console.error('❌ Помилка парсингу roadmap JSON:', e);
            alert('Сервер повернув некоректний формат roadmap');
          }
        },
        error: (err: any) => {
          console.error('❌ Помилка при генерації roadmap:', err);
          alert('Запит не вдався');
        }
      });
    } catch (err: any) {
      alert('❌ Некоректний вхідний JSON');
    }
  }

  sendRequest(parsed: any) {
    const payload = {
      ...parsed,
      model: this.selectedModel
    };
    this.isLoading = true;

    this.roadmapService.generateRoadmap(payload).subscribe({
      next: (response) => {
        const roadmap = JSON.parse(response.roadmap);
        this.buildGraphFromRoadmap(roadmap);
        this.isLoading = false;
      },
      error: (err) => {
        alert('Помилка при запиті');
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  buildGraphFromRoadmap(roadmap: any) {
    this.nodes = [];
    this.links = [];

    for (const step of roadmap.steps) {
      const stepId = `step-${step.step_number}`;
      this.nodes.push({ id: stepId, label: step.title });

      for (const topic of step.topics) {
        const topicId = `${stepId}-${topic.concept}`;
        this.nodes.push({ id: topicId, label: topic.concept });
        this.links.push({ source: stepId, target: topicId });

        for (const sub of topic.subtopics) {
          const subId = `${topicId}-${sub}`;
          this.nodes.push({ id: subId, label: sub });
          this.links.push({ source: topicId, target: subId });
        }
      }
    }
  }
  
}