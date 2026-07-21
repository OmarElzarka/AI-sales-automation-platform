import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
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
  imports: [CommonModule, DragDropModule],
  templateUrl: './deal-board.component.html',
  styleUrls: ['./deal-board.component.scss']
})
export class DealBoardComponent implements OnInit {
  columns: Column[] = [];
  loading = true;
  error: string | null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadDeals();
  }

  loadDeals() {
    this.loading = true;
    this.error = null;
    this.apiService.get<any>('/deals/pipeline').subscribe({
      next: (data: any) => {
        // Backend returns Dictionary<string, List<DealDto>> which translates to an object in JS
        const stages = ['Lead', 'Qualified', 'Proposal', 'Negotiation', 'Won', 'Lost'];
        this.columns = stages.map(stage => ({
          id: stage,
          title: stage,
          deals: data[stage] || []
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load deals pipeline:', err);
        this.error = 'Failed to load pipeline data.';
        this.columns = [];
        this.loading = false;
      }
    });
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
      const newStage = event.container.id; // Map container id to stage string
      
      // Update backend
      this.apiService.put(`/deals/${movedDeal.id}/stage`, { dealId: movedDeal.id, toStage: newStage }).subscribe({
        error: (err) => {
          console.error('Failed to update deal stage:', err);
          // Revert if failed (simple reload for now)
          this.loadDeals();
        }
      });
    }
  }
}
