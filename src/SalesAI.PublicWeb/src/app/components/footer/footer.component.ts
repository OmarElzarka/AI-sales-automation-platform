import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PublicApiService } from '../../services/public-api.service';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  newsletterEmail = '';
  newsletterMessage = '';
  newsletterSuccess = false;
  isSubmitting = false;
  currentYear = new Date().getFullYear();

  constructor(private api: PublicApiService) {}

  submitNewsletter() {
    if (!this.newsletterEmail || this.isSubmitting) return;

    this.isSubmitting = true;
    this.api.subscribeNewsletter({ email: this.newsletterEmail }).subscribe({
      next: (res) => {
        this.newsletterMessage = res.message;
        this.newsletterSuccess = true;
        this.newsletterEmail = '';
        this.isSubmitting = false;
      },
      error: () => {
        this.newsletterMessage = 'Something went wrong. Please try again.';
        this.newsletterSuccess = false;
        this.isSubmitting = false;
      }
    });
  }
}
