import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyDataTopicApplyComponent } from './myApply.component';

describe('MyDataTopicApplyComponent', () => {
  let component: MyDataTopicApplyComponent;
  let fixture: ComponentFixture<MyDataTopicApplyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyDataTopicApplyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyDataTopicApplyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
