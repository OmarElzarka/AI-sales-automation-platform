import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-email-composer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './email-composer.component.html',
  styleUrls: ['./email-composer.component.scss']
})
export class EmailComposerComponent implements OnInit {
  emailForm: FormGroup;
  generating = false;
  sending = false;

  constructor(private fb: FormBuilder, private apiService: ApiService) {
    this.emailForm = this.fb.group({
      to: ['', [Validators.required]],
      subject: ['', Validators.required],
      body: ['', Validators.required],
      tone: ['professional'] // internal state for AI generation
    });
  }

  ngOnInit() {}

  generateAIEmail() {
    const to = this.emailForm.get('to')?.value;
    if (!to) {
      alert("Please enter a recipient to generate contextual email.");
      return;
    }

    this.generating = true;
    
    // Call the actual AI email generation endpoint
    this.apiService.post('/ai/email/generate', { 
      recipientEmail: to, 
      tone: this.emailForm.get('tone')?.value || 'professional'
    }).subscribe({
      next: (data: any) => {
        this.emailForm.patchValue({
          subject: data.subject || 'Automated Outreach',
          body: data.body || data.content || ''
        });
        this.generating = false;
      },
      error: (err) => {
        console.error('Failed to generate AI email:', err);
        alert('Failed to generate AI email. Please check your API key.');
        this.generating = false;
      }
    });
  }

  setTone(tone: string) {
    this.emailForm.patchValue({ tone });
    if (this.emailForm.get('to')?.value) {
      this.generateAIEmail(); // Regenerate with new tone
    }
  }

  sendEmail() {
    if (this.emailForm.invalid) return;

    this.sending = true;
    
    // Call the actual email sending endpoint
    const payload = this.emailForm.value;
    this.apiService.post('/outreach/email/send', payload).subscribe({
      next: () => {
        alert('Email sent successfully!');
        this.sending = false;
        this.emailForm.reset({ tone: 'professional' });
      },
      error: (err) => {
        console.error('Failed to send email:', err);
        alert('Failed to send email.');
        this.sending = false;
      }
    });
  }
}
