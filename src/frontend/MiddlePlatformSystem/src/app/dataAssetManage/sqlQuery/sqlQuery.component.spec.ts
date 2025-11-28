import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SqlqueryComponent } from './sqlQuery.component';

describe('SqlqueryComponent', () => {
  let component: SqlqueryComponent;
  let fixture: ComponentFixture<SqlqueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SqlqueryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SqlqueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
