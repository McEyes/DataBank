import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicRecordComponent } from './record.component';

describe('DataTopicRecordComponent', () => {
  let component: DataTopicRecordComponent;
  let fixture: ComponentFixture<DataTopicRecordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataTopicRecordComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataTopicRecordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
