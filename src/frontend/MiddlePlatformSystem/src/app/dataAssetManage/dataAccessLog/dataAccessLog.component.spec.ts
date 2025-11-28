import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataapilogComponent } from './dataAccessLog.component';

describe('DataapilogComponent', () => {
  let component: DataapilogComponent;
  let fixture: ComponentFixture<DataapilogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataapilogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataapilogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
