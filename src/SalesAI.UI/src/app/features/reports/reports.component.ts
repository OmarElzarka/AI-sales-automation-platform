import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  aiInsights: string[] = [];
  metrics: any = null;
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadReports();
  }

  loadReports() {
    this.apiService.get('/reports').subscribe({
      next: (data: any) => {
        this.metrics = data.metrics;
        this.aiInsights = data.insights;
        this.loading = false;
      },
      error: () => {
        // Fallback mocked data
        this.metrics = {
          totalRevenue: 2450000,
          avgDealSize: 34000,
          salesCycleDays: 42,
          winRate: 68.5,
          topPerformers: [
            { name: 'Sarah Jenkins', revenue: 850000 },
            { name: 'Mike Ross', revenue: 620000 }
          ],
          revenueByMonth: [120, 150, 180, 140, 210, 245]
        };
        this.aiInsights = [
          "Win rate drops by 15% when sales cycles extend beyond 45 days. Consider implementing automated re-engagement triggers at day 30.",
          "Deals originating from outbound email campaigns have a 20% higher average deal size compared to inbound leads.",
          "Top performer Sarah Jenkins averages 3 extra touchpoints per deal, mostly phone calls, leading to a 85% win rate in Enterprise deals."
        ];
        this.loading = false;
      }
    });
  }
}
