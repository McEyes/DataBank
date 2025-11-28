import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataAuthDetailComponent } from './data-auth-detail.component';

describe('DataAuthDetailComponent', () => {
  let component: DataAuthDetailComponent;
  let fixture: ComponentFixture<DataAuthDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataAuthDetailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataAuthDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
