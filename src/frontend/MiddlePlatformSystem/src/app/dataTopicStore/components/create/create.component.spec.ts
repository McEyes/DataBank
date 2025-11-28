import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicEditHomeComponent } from './create.component';

describe('DataTopicEditHomeComponent', () => {
  let component: DataTopicEditHomeComponent;
  let fixture: ComponentFixture<DataTopicEditHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DataTopicEditHomeComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DataTopicEditHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
