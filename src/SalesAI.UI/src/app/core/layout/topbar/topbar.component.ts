import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

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
  searchResults: any[] = [];
  private searchSubject = new Subject<string>();

  constructor(private apiService: ApiService) {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.performSearch(query);
    });
  }

  onSearch() {
    this.searchSubject.next(this.searchQuery);
  }

  performSearch(query: string) {
    if (query.length === 0) {
      this.showResults = false;
      this.searchResults = [];
      return;
    }
    this.apiService.get(`/search?q=${encodeURIComponent(query)}`).subscribe({
      next: (data: any) => {
        this.searchResults = data || [];
        this.showResults = true;
      },
      error: (err) => console.error('Search failed:', err)
    });
  }

  closeSearch() {
    setTimeout(() => this.showResults = false, 200);
  }
}

