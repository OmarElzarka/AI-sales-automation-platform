import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PublicApiService, PublicLeadRequest } from '../../services/public-api.service';

@Component({
  selector: 'app-book-demo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-demo.component.html',
  styleUrl: './book-demo.component.scss'
})
export class BookDemoComponent {
  isSubmitting = false;
  isSuccess = false;
  successMessage = '';
  errorMessage = '';

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

  constructor(private api: PublicApiService) {}

  submitForm() {
    if (this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    this.api.submitDemoRequest(this.form).subscribe({
      next: (res) => {
        this.isSuccess = true;
        this.successMessage = res.message;
        this.isSubmitting = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.detail || 'Something went wrong. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}
