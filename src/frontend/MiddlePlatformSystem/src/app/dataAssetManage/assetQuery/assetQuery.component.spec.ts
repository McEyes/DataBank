import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetqueryComponent } from './assetquery.component';

describe('AssetqueryComponent', () => {
  let component: AssetqueryComponent;
  let fixture: ComponentFixture<AssetqueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssetqueryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssetqueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
