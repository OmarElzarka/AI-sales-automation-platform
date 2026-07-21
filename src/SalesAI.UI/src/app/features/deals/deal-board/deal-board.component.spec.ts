import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DealBoardComponent } from './deal-board.component';

describe('DealBoardComponent', () => {
  let component: DealBoardComponent;
  let fixture: ComponentFixture<DealBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DealBoardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DealBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
