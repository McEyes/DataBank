import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusinessModelCategoryManageComponent } from './categoryManage.component';

describe('BusinessModelCategoryManageComponent', () => {
  let component: BusinessModelCategoryManageComponent;
  let fixture: ComponentFixture<BusinessModelCategoryManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BusinessModelCategoryManageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusinessModelCategoryManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
