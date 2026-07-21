import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent {
  userInitial = 'O'; // Mocked for now
  searchQuery = '';
  showResults = false;

  mockResults = [
    { type: 'Lead', name: 'John Doe', sub: 'Acme Corp', route: '/leads/1' },
    { type: 'Deal', name: 'Enterprise License', sub: '$25,000', route: '/deals' }
  ];

  onSearch() {
    this.showResults = this.searchQuery.length > 0;
  }

  closeSearch() {
    setTimeout(() => this.showResults = false, 200);
  }
}

