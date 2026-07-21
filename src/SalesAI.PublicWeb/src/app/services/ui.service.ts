import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UiService {
  private demoModalOpenSource = new Subject<boolean>();
  demoModalState$ = this.demoModalOpenSource.asObservable();

  openDemoModal() {
    this.demoModalOpenSource.next(true);
    document.body.style.overflow = 'hidden';
  }

  closeDemoModal() {
    this.demoModalOpenSource.next(false);
    document.body.style.overflow = '';
  }
}
