import { Component } from '@angular/core';
import { NgxGraphModule } from '@swimlane/ngx-graph';
import { RoadmapGraphComponent } from './components/roadmap-graph/roadmap-graph.component';

@Component({
  selector: 'app-root',
  imports: [NgxGraphModule, RoadmapGraphComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'roadmap-visualizer';
}
