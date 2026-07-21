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
    
    // Simulate API call for AI generation
    setTimeout(() => {
      this.emailForm.patchValue({
        subject: 'Enhance your engineering team\'s productivity',
        body: `Hi there,\n\nI noticed your recent scaling efforts and thought SalesAI could help streamline your team's workflow. Our AI-driven platform reduces manual tasks by 40%.\n\nWould you be open to a brief 10-minute chat next Tuesday to see if it makes sense for your team?\n\nBest,\nOmar`
      });
      this.generating = false;
    }, 1500);
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
    
    // Simulate API call
    setTimeout(() => {
      alert('Email sent successfully!');
      this.sending = false;
      this.emailForm.reset({ tone: 'professional' });
    }, 1000);
  }
}
