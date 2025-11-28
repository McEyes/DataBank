import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicComponent } from './dataTopic.component';

describe('DataTopicComponent', () => {
  let component: DataTopicComponent;
  let fixture: ComponentFixture<DataTopicComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataTopicComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataTopicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
