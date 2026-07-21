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

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadLeads();
  }

  loadLeads() {
    this.apiService.get('/leads').subscribe({
      next: (data: any) => {
        this.dataSource = data.items || data;
        this.loading = false;
      },
      error: () => {
        // Mock data for UI development
        this.dataSource = [
          { id: '1', firstName: 'John', lastName: 'Doe', companyName: 'Acme Corp', email: 'john@acme.com', status: 'New', scoreCategory: 'Hot', scoreNumeric: 95 },
          { id: '2', firstName: 'Jane', lastName: 'Smith', companyName: 'TechFlow', email: 'jane@techflow.io', status: 'Contacted', scoreCategory: 'Warm', scoreNumeric: 65 },
          { id: '3', firstName: 'Bob', lastName: 'Johnson', companyName: 'Global Inc', email: 'bob@global.com', status: 'Qualified', scoreCategory: 'Cold', scoreNumeric: 25 },
        ];
        this.loading = false;
      }
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
      default: return 'bg-surface-100 text-surface-800 border-surface-200';
    }
  }
}
