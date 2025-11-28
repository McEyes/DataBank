import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyDataTopicComponent } from './myData.component';

describe('MyDataTopicComponent', () => {
  let component: MyDataTopicComponent;
  let fixture: ComponentFixture<MyDataTopicComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyDataTopicComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyDataTopicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
