import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicStoreHomeComponent } from './home.component';

describe('DataTopicStoreHomeComponent', () => {
  let component: DataTopicStoreHomeComponent;
  let fixture: ComponentFixture<DataTopicStoreHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DataTopicStoreHomeComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DataTopicStoreHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
