import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FindAProviderComponent } from './find-a-provider.component';

describe('FindAProviderComponent', () => {
  let component: FindAProviderComponent;
  let fixture: ComponentFixture<FindAProviderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FindAProviderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FindAProviderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
