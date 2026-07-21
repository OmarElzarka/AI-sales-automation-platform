import { Component, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PublicApiService, PublicLeadRequest } from '../../services/public-api.service';

@Component({
  selector: 'app-request-demo-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './request-demo-modal.component.html',
  styleUrl: './request-demo-modal.component.scss'
})
export class RequestDemoModalComponent {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();

  isSubmitting = false;
  isSuccess = false;
  errorMessage = '';
  successMessage = '';

  form: PublicLeadRequest = {
    firstName: '',
    lastName: '',
    company: '',
    businessEmail: '',
    phone: '',
    industry: '',
    companySize: '',
    country: '',
    interestedProduct: '',
    estimatedBudget: '',
    message: ''
  };

  industries = [
    'Technology', 'Healthcare', 'Finance', 'Education', 'Retail',
    'Manufacturing', 'Real Estate', 'Media', 'Consulting', 'Other'
  ];

  companySizes = [
    { label: '1-10 employees', value: 'Startup' },
    { label: '11-50 employees', value: 'Small' },
    { label: '51-500 employees', value: 'Medium' },
    { label: '500+ employees', value: 'Enterprise' }
  ];

  countries = [
    'United States', 'United Kingdom', 'Canada', 'Germany', 'France',
    'Australia', 'Netherlands', 'Sweden', 'Singapore', 'Japan',
    'India', 'Brazil', 'UAE', 'Other'
  ];

  products = [
    'Workflow Automation', 'AI Analytics', 'Team Collaboration',
    'Enterprise Suite', 'Custom Integration', 'Full Platform'
  ];

  budgets = [
    'Under $1,000/mo', '$1,000 - $5,000/mo', '$5,000 - $10,000/mo',
    '$10,000 - $25,000/mo', '$25,000+/mo', 'Not sure yet'
  ];

  constructor(private api: PublicApiService) {}

  onOverlayClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.closeModal();
    }
  }

  closeModal() {
    this.close.emit();
    // Reset after animation
    setTimeout(() => {
      this.isSuccess = false;
      this.errorMessage = '';
    }, 300);
  }

  submitForm() {
    if (this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    this.api.submitDemoRequest(this.form).subscribe({
      next: (res) => {
        this.isSuccess = true;
        this.successMessage = res.message;
        this.isSubmitting = false;
        this.resetForm();
      },
      error: (err) => {
        this.errorMessage = err.error?.detail || err.error?.message || 'Something went wrong. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  private resetForm() {
    this.form = {
      firstName: '',
      lastName: '',
      company: '',
      businessEmail: '',
      phone: '',
      industry: '',
      companySize: '',
      country: '',
      interestedProduct: '',
      estimatedBudget: '',
      message: ''
    };
  }
}
