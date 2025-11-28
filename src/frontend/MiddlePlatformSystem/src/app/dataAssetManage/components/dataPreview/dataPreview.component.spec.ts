import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DatapreviewComponent } from './datapreview.component';

describe('DatapreviewComponent', () => {
  let component: DatapreviewComponent;
  let fixture: ComponentFixture<DatapreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DatapreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DatapreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
