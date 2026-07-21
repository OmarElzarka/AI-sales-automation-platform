import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  kpis: any = null;
  activities: any[] = [];
  funnel: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    // In a real app, we would use forkJoin or individual requests.
    // For now, let's mock some data if the backend isn't ready or just try to fetch.
    this.apiService.get('/dashboard/kpis').subscribe({
      next: (data: any) => {
        this.kpis = data;
        this.loading = false;
      },
      error: () => {
        // Fallback mock data for visual demonstration
        this.kpis = { totalLeads: 142, activeDeals: 28, winRate: 65.4, revenuePipeline: 450000 };
        this.activities = [
          { type: 'LeadCreated', title: 'New Lead: John Doe', createdAt: new Date().toISOString() },
          { type: 'DealWon', title: 'Closed: Acme Corp', createdAt: new Date().toISOString() },
          { type: 'Meeting', title: 'Discovery Call with TechCorp', createdAt: new Date().toISOString() }
        ];
        this.funnel = [
          { stage: 'NewLead', count: 45 },
          { stage: 'Qualified', count: 30 },
          { stage: 'Proposal', count: 15 },
          { stage: 'Negotiation', count: 8 },
          { stage: 'Won', count: 5 }
        ];
        this.loading = false;
      }
    });
  }

  getActivityIcon(type: string): string {
    switch (type) {
      case 'LeadCreated': return 'person_add';
      case 'DealWon': return 'emoji_events';
      case 'Meeting': return 'event';
      case 'Note': return 'note';
      default: return 'notifications';
    }
  }

  getActivityColor(type: string): string {
    switch (type) {
      case 'LeadCreated': return 'text-blue-500 bg-blue-100';
      case 'DealWon': return 'text-green-500 bg-green-100';
      case 'Meeting': return 'text-purple-500 bg-purple-100';
      case 'Note': return 'text-yellow-500 bg-yellow-100';
      default: return 'text-gray-500 bg-gray-100';
    }
  }
}
