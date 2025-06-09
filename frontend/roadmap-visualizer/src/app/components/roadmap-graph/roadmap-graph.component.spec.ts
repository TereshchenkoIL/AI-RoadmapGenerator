import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoadmapGraphComponent } from './roadmap-graph.component';

describe('RoadmapGraphComponent', () => {
  let component: RoadmapGraphComponent;
  let fixture: ComponentFixture<RoadmapGraphComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoadmapGraphComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoadmapGraphComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
