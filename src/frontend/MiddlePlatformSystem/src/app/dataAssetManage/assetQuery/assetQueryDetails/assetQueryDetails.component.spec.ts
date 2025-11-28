import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetquerydetailsComponent } from './assetquerydetails.component';

describe('AssetquerydetailsComponent', () => {
  let component: AssetquerydetailsComponent;
  let fixture: ComponentFixture<AssetquerydetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssetquerydetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssetquerydetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
