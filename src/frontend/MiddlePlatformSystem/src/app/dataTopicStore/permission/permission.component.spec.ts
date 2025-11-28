import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTopicPermissionComponent } from './permission.component';

describe('DataTopicPermissionComponent', () => {
  let component: DataTopicPermissionComponent;
  let fixture: ComponentFixture<DataTopicPermissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataTopicPermissionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataTopicPermissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
