import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataAuthorizedComponent } from './dataAuthorized.component';

describe('DataAuthorizedComponent', () => {
  let component: DataAuthorizedComponent;
  let fixture: ComponentFixture<DataAuthorizedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataAuthorizedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataAuthorizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
