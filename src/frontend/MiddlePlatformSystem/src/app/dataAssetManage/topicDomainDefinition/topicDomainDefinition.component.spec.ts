import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopiccategoryComponent } from './topicDomainDefinition.component';

describe('TopiccategoryComponent', () => {
  let component: TopiccategoryComponent;
  let fixture: ComponentFixture<TopiccategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TopiccategoryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopiccategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
