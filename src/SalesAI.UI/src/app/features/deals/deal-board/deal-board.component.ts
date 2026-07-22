import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

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
  imports: [CommonModule, DragDropModule, FormsModule],
  templateUrl: './deal-board.component.html',
  styleUrls: ['./deal-board.component.scss']
})
export class DealBoardComponent implements OnInit {
  columns: Column[] = [];
  loading = true;
  error: string | null = null;
  
  showNewDealModal = false;
  submitting = false;
  newDeal = {
    title: '',
    value: 0,
    currency: 'USD',
    stage: 'NewLead',
    probability: 50,
    ownerId: ''
  };

  constructor(private apiService: ApiService, private authService: AuthService) {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      const user = JSON.parse(userStr);
      this.newDeal.ownerId = user.id || '00000000-0000-0000-0000-000000000001'; // Default admin
    } else {
      this.newDeal.ownerId = '00000000-0000-0000-0000-000000000001'; // Fallback
    }
  }

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

  openNewDealModal() {
    this.showNewDealModal = true;
    this.newDeal.title = '';
    this.newDeal.value = 0;
  }

  closeNewDealModal() {
    this.showNewDealModal = false;
  }

  submitNewDeal() {
    if (!this.newDeal.title || this.newDeal.value <= 0) {
      alert('Title and Value > 0 are required.');
      return;
    }
    
    this.submitting = true;
    this.apiService.post('/deals', this.newDeal).subscribe({
      next: () => {
        this.submitting = false;
        this.showNewDealModal = false;
        this.loadDeals();
      },
      error: (err) => {
        console.error('Failed to create deal:', err);
        alert('Failed to create deal.');
        this.submitting = false;
      }
    });
  }

  summarizeMeeting(deal: Deal) {
    const transcript = prompt(`Enter transcript or notes for deal ${deal.title}:`);
    if (!transcript) return;
    
    this.apiService.post(`/ai/deals/${deal.id}/summarize-meeting`, `"${transcript}"`).subscribe({
      next: (data: any) => {
        alert('AI Meeting Summary:\n' + (data.summary || data.content || 'Check console for details'));
        console.log('Summary Result:', data);
      },
      error: (err) => {
        console.error('Failed to summarize meeting:', err);
        alert('Failed to generate summary. Check API key.');
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
