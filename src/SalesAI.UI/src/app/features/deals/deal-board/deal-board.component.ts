import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { ApiService } from '../../../core/services/api.service';

interface Deal {
  id: string;
  title: string;
  companyName: string;
  value: number;
  contactName: string;
  aiProbability: number;
}

interface Column {
  id: string;
  title: string;
  deals: Deal[];
}

@Component({
  selector: 'app-deal-board',
  standalone: true,
  imports: [CommonModule, DragDropModule, MatCardModule, MatIconModule, MatButtonModule, MatBadgeModule],
  templateUrl: './deal-board.component.html',
  styleUrls: ['./deal-board.component.css']
})
export class DealBoardComponent implements OnInit {
  columns: Column[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadDeals();
  }

  loadDeals() {
    this.apiService.get('/deals').subscribe({
      next: (data: any) => {
        this.columns = this.groupDeals(data);
        this.loading = false;
      },
      error: () => {
        // Mock data
        this.columns = [
          {
            id: 'Lead', title: 'Lead', deals: [
              { id: '1', title: 'Enterprise License', companyName: 'Acme Corp', value: 25000, contactName: 'John Doe', aiProbability: 35 },
              { id: '2', title: 'Basic Setup', companyName: 'Startup Inc', value: 5000, contactName: 'Jane Smith', aiProbability: 60 }
            ]
          },
          {
            id: 'Qualified', title: 'Qualified', deals: [
              { id: '3', title: 'Data Migration', companyName: 'TechFlow', value: 12000, contactName: 'Bob Johnson', aiProbability: 75 }
            ]
          },
          {
            id: 'Proposal', title: 'Proposal', deals: [
              { id: '4', title: 'Custom Implementation', companyName: 'Global Co', value: 45000, contactName: 'Alice Brown', aiProbability: 90 }
            ]
          },
          {
            id: 'Won', title: 'Won', deals: []
          }
        ];
        this.loading = false;
      }
    });
  }

  groupDeals(deals: any[]): Column[] {
    const defaultCols: Column[] = [
      { id: 'Lead', title: 'Lead', deals: [] },
      { id: 'Qualified', title: 'Qualified', deals: [] },
      { id: 'Proposal', title: 'Proposal', deals: [] },
      { id: 'Won', title: 'Won', deals: [] }
    ];
    // Normally loop through deals and push to respective columns
    return defaultCols;
  }

  drop(event: CdkDragDrop<Deal[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      
      const movedDeal = event.container.data[event.currentIndex];
      const newStage = event.container.id; // Usually mapped from column data
      
      // Update backend
      // this.apiService.put(`/deals/${movedDeal.id}/stage`, { stage: newStage }).subscribe();
    }
  }
}
