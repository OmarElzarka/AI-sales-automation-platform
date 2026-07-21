import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
  timeline: any[] = [];
  loading = true;
  error: string | null = null;
  scoringInProgress = false;
  emailGenerating = false;
  playbookGenerating = false;
  generatedEmail: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService
  ) {}

  ngOnInit() {
    this.leadId = this.route.snapshot.paramMap.get('id');
    if (this.leadId) {
      this.loadLeadDetails();
      this.loadTimeline();
    }
  }

  loadLeadDetails() {
    this.loading = true;
    this.apiService.get(`/leads/${this.leadId}`).subscribe({
      next: (data: any) => {
        this.lead = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load lead:', err);
        this.error = 'Failed to load lead details.';
        this.loading = false;
      }
    });
  }

  loadTimeline() {
    this.apiService.get(`/leads/${this.leadId}/timeline`).subscribe({
      next: (data: any) => {
        this.timeline = data || [];
      },
      error: () => {
        this.timeline = [];
      }
    });
  }

  scoreLead() {
    this.scoringInProgress = true;
    this.apiService.post(`/ai/leads/${this.leadId}/score`, {}).subscribe({
      next: (data: any) => {
        this.lead.scoreCategory = data.category;
        this.lead.scoreNumeric = data.numericScore;
        this.lead.scoreReasoning = data.reasoning;
        this.scoringInProgress = false;
        this.loadTimeline();
      },
      error: (err) => {
        console.error('AI scoring failed:', err);
        this.scoringInProgress = false;
        alert('AI scoring failed. Check that the Gemini API key is configured.');
      }
    });
  }

  generateEmail() {
    this.emailGenerating = true;
    this.generatedEmail = null;
    this.apiService.post(`/ai/leads/${this.leadId}/generate-email`, {}).subscribe({
      next: (data: any) => {
        this.generatedEmail = data;
        this.emailGenerating = false;
      },
      error: (err) => {
        console.error('Email generation failed:', err);
        this.emailGenerating = false;
        alert('Email generation failed. Check that the Gemini API key is configured.');
      }
    });
  }

  generatePlaybook() {
    this.playbookGenerating = true;
    this.apiService.post(`/ai/leads/${this.leadId}/playbook`, {}).subscribe({
      next: (data: any) => {
        this.lead.aiPlaybook = data;
        this.playbookGenerating = false;
      },
      error: (err) => {
        console.error('Playbook generation failed:', err);
        this.playbookGenerating = false;
        alert('Playbook generation failed. Check that the Gemini API key is configured.');
      }
    });
  }

  deleteLead() {
    if (!confirm('Are you sure you want to delete this lead?')) return;
    this.apiService.delete(`/leads/${this.leadId}`).subscribe({
      next: () => this.router.navigate(['/leads']),
      error: (err) => console.error('Delete failed:', err)
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
}

