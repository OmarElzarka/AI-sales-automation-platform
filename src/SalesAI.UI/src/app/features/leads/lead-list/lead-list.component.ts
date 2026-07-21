import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-lead-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    MatTableModule, 
    MatPaginatorModule, 
    MatSortModule, 
    MatInputModule, 
    MatButtonModule, 
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './lead-list.component.html',
  styleUrls: ['./lead-list.component.css']
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
      case 'Hot': return 'bg-red-100 text-red-800';
      case 'Warm': return 'bg-yellow-100 text-yellow-800';
      case 'Cold': return 'bg-blue-100 text-blue-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'New': return 'bg-purple-100 text-purple-800';
      case 'Contacted': return 'bg-blue-100 text-blue-800';
      case 'Qualified': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
