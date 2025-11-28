import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PermissionApplicationRecordComponent } from './permissionApplicationRecord.component';

describe('PermissionApplicationRecordComponent', () => {
  let component: PermissionApplicationRecordComponent;
  let fixture: ComponentFixture<PermissionApplicationRecordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PermissionApplicationRecordComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PermissionApplicationRecordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
