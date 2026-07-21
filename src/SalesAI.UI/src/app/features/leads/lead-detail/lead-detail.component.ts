import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-lead-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './lead-detail.component.html',
  styleUrls: ['./lead-detail.component.scss']
})
export class LeadDetailComponent implements OnInit {
  leadId: string | null = null;
  lead: any = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService
  ) {}

  ngOnInit() {
    this.leadId = this.route.snapshot.paramMap.get('id');
    if (this.leadId) {
      this.loadLeadDetails();
    }
  }

  loadLeadDetails() {
    this.apiService.get(`/leads/${this.leadId}`).subscribe({
      next: (data: any) => {
        this.lead = data;
        this.loading = false;
      },
      error: () => {
        // Mock data
        this.lead = {
          id: this.leadId,
          firstName: 'John',
          lastName: 'Doe',
          email: 'john@acme.com',
          jobTitle: 'VP of Engineering',
          companyName: 'Acme Corp',
          status: 'Qualified',
          scoreCategory: 'Hot',
          scoreNumeric: 95,
          scoreReasoning: 'High intent shown based on recent product demo attendance and pricing page visits. Matches ICP perfectly.',
          activities: [
            { type: 'Meeting', title: 'Product Demo', date: new Date().toISOString() },
            { type: 'Email', title: 'Sent pricing proposal', date: new Date(Date.now() - 86400000).toISOString() }
          ],
          aiPlaybook: null
        };
        this.loading = false;
      }
    });
  }

  generatePlaybook() {
    // Generate playbook via API
    this.apiService.post(`/ai/leads/${this.leadId}/playbook`, {}).subscribe({
      next: (data: any) => {
        this.lead.aiPlaybook = data;
      },
      error: () => {
        this.lead.aiPlaybook = {
          bestChannel: { channel: 'LinkedIn', reasoning: 'High activity on technical posts' },
          salesApproach: { strategy: 'Value-based selling focusing on engineering efficiency', openingLine: 'Hi John, saw your recent post about scaling engineering teams...' },
          expectedObjections: [{ objection: 'Too expensive', response: 'Focus on ROI and time saved per developer.' }]
        };
      }
    });
  }
}
