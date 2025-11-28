import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataUserauthorityqueryComponent } from './userPermissionApplicationRecord.component';

describe('DataUserauthorityqueryComponent', () => {
  let component: DataUserauthorityqueryComponent;
  let fixture: ComponentFixture<DataUserauthorityqueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataUserauthorityqueryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataUserauthorityqueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
