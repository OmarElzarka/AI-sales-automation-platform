import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  kpis: any = null;
  activities: any[] = [];
  funnel: any[] = [];
  leadSources: any[] = [];
  upcomingMeetings: any[] = [];
  loading = true;
  error: string | null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.error = null;

    forkJoin({
      dashboard: this.apiService.get<any>('/dashboard/data'),
      activities: this.apiService.get<any[]>('/dashboard/activity', { take: 10 }),
      tasks: this.apiService.get<any[]>('/tasks')
    }).subscribe({
      next: ({ dashboard, activities, tasks }) => {
        this.kpis = {
          totalLeads: dashboard.kpis.totalLeads,
          activeDeals: dashboard.kpis.totalDeals,
          winRate: dashboard.kpis.winRatePercentage,
          revenuePipeline: dashboard.kpis.totalRevenue,
          activeTasks: dashboard.kpis.activeTasks
        };
        this.funnel = (dashboard.pipelineFunnel || []).filter((s: any) => s.stageName !== 'Won' && s.stageName !== 'Lost').map((s: any) => ({
          stage: s.stageName,
          count: s.count,
          value: s.totalValue
        }));
        this.leadSources = dashboard.leadSources || [];
        this.activities = activities || [];
        
        const allTasks = (tasks as any).items || tasks || [];
        this.upcomingMeetings = allTasks.filter((t: any) => t.type === 'Meeting' && !t.isCompleted);
        
        this.loading = false;
      },
      error: (err) => {
        console.error('Dashboard load error:', err);
        this.error = 'Failed to load dashboard data.';
        this.loading = false;
      }
    });
  }

  getActivityIcon(type: string): string {
    switch (type) {
      case 'LeadCreated': return 'person_add';
      case 'LeadScored': return 'psychology';
      case 'DealWon': return 'emoji_events';
      case 'Meeting': return 'event';
      case 'Note': return 'note';
      case 'Email': return 'email';
      case 'Call': return 'phone';
      default: return 'notifications';
    }
  }

  getActivityColor(type: string): string {
    switch (type) {
      case 'LeadCreated': return 'bg-brand-500';
      case 'LeadScored': return 'bg-purple-500';
      case 'DealWon': return 'bg-emerald-500';
      case 'Meeting': return 'bg-indigo-500';
      case 'Note': return 'bg-amber-500';
      case 'Email': return 'bg-sky-500';
      case 'Call': return 'bg-teal-500';
      default: return 'bg-surface-400';
    }
  }
}

