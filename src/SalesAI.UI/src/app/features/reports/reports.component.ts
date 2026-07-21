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
  error: string | null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadReports();
  }

  loadReports() {
    this.loading = true;
    this.error = null;
    this.apiService.get('/reports').subscribe({
      next: (data: any) => {
        this.metrics = data.metrics || data;
        this.aiInsights = data.insights || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load reports:', err);
        this.error = 'Failed to load report data.';
        this.loading = false;
      }
    });
  }

  exportCsv() {
    this.apiService.get('/reports/export-csv', { responseType: 'blob' } as any).subscribe({
      next: (blob: any) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'sales_report.csv';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Failed to export CSV:', err)
    });
  }

  exportPdf() {
    this.apiService.get('/reports/export-pdf', { responseType: 'blob' } as any).subscribe({
      next: (blob: any) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'sales_report.pdf';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Failed to export PDF:', err)
    });
  }
}
