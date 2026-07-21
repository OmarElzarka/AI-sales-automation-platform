import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-lead-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule
  ],
  templateUrl: './lead-list.component.html',
  styleUrls: ['./lead-list.component.scss']
})
export class LeadListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'company', 'email', 'status', 'score', 'actions'];
  dataSource: any[] = [];
  loading = true;
  error: string | null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadLeads();
  }

  loadLeads() {
    this.loading = true;
    this.error = null;
    this.apiService.get('/leads').subscribe({
      next: (data: any) => {
        this.dataSource = data.items || data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load leads:', err);
        this.error = 'Failed to load leads.';
        this.dataSource = [];
        this.loading = false;
      }
    });
  }

  deleteLead(id: string) {
    if (!confirm('Are you sure you want to delete this lead?')) return;
    this.apiService.delete(`/leads/${id}`).subscribe({
      next: () => this.loadLeads(),
      error: (err) => console.error('Delete failed:', err)
    });
  }

  getScoreColor(category: string): string {
    switch (category) {
      case 'Hot': return 'bg-rose-100 text-rose-800 border-rose-200';
      case 'Warm': return 'bg-amber-100 text-amber-800 border-amber-200';
      case 'Cold': return 'bg-sky-100 text-sky-800 border-sky-200';
      default: return 'bg-surface-100 text-surface-800 border-surface-200';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'New': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'Contacted': return 'bg-brand-100 text-brand-800 border-brand-200';
      case 'Qualified': return 'bg-emerald-100 text-emerald-800 border-emerald-200';
      case 'Unqualified': return 'bg-surface-100 text-surface-800 border-surface-200';
      case 'Converted': return 'bg-teal-100 text-teal-800 border-teal-200';
      case 'Lost': return 'bg-rose-100 text-rose-800 border-rose-200';
      default: return 'bg-surface-100 text-surface-800 border-surface-200';
    }
  }
}

