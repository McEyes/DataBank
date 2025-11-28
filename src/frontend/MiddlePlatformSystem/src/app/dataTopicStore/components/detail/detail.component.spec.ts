import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicStoreDetailComponent } from './detail.component';

describe('DataTopicStoreDetailComponent', () => {
  let component: DataTopicStoreDetailComponent;
  let fixture: ComponentFixture<DataTopicStoreDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DataTopicStoreDetailComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DataTopicStoreDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
