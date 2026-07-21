import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PublicApiService, ContactRequest } from '../../services/public-api.service';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss'
})
export class ContactComponent {
  activeTab: 'general' | 'sales' | 'support' = 'sales';
  isSubmitting = false;
  isSuccess = false;
  successMessage = '';
  errorMessage = '';

  form: ContactRequest = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    company: '',
    inquiryType: 'Sales',
    subject: '',
    message: ''
  };

  constructor(private api: PublicApiService) {}

  setTab(tab: 'general' | 'sales' | 'support') {
    this.activeTab = tab;
    this.form.inquiryType = tab.charAt(0).toUpperCase() + tab.slice(1);
    this.isSuccess = false;
    this.errorMessage = '';
  }

  submitForm() {
    if (this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    this.api.submitContactForm(this.form).subscribe({
      next: (res) => {
        this.isSuccess = true;
        this.successMessage = res.message;
        this.isSubmitting = false;
        this.resetForm();
      },
      error: (err) => {
        this.errorMessage = err.error?.detail || 'Something went wrong. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  private resetForm() {
    this.form = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      company: '',
      inquiryType: this.activeTab.charAt(0).toUpperCase() + this.activeTab.slice(1),
      subject: '',
      message: ''
    };
  }
}
