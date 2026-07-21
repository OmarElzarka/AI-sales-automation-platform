import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  navItems = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Leads', icon: 'people', route: '/leads' },
    { label: 'Deals', icon: 'view_kanban', route: '/deals' },
    { label: 'Tasks', icon: 'check_circle', route: '/tasks' },
    { label: 'Outreach', icon: 'email', route: '/outreach' },
    { label: 'Reports', icon: 'bar_chart', route: '/reports' },
  ];

  constructor(private authService: AuthService, private router: Router) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
