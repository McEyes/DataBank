import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataauthorityComponent } from './dataAuthority.component';

describe('DataauthorityComponent', () => {
  let component: DataauthorityComponent;
  let fixture: ComponentFixture<DataauthorityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataauthorityComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataauthorityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
