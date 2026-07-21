import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { RequestDemoModalComponent } from './components/request-demo-modal/request-demo-modal.component';
import { UiService } from './services/ui.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent, RequestDemoModalComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  isDemoModalOpen = false;
  private subscription?: Subscription;

  constructor(private uiService: UiService) {}

  ngOnInit() {
    this.subscription = this.uiService.demoModalState$.subscribe(isOpen => {
      this.isDemoModalOpen = isOpen;
    });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  closeDemoModal() {
    this.uiService.closeDemoModal();
  }
}
