import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PublicLeadRequest {
  firstName: string;
  lastName: string;
  company: string;
  businessEmail: string;
  phone?: string;
  industry?: string;
  companySize?: string;
  country?: string;
  interestedProduct?: string;
  estimatedBudget?: string;
  message?: string;
}

export interface ContactRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  company?: string;
  inquiryType: string;
  subject: string;
  message: string;
}

export interface NewsletterRequest {
  email: string;
}

export interface ApiResponse<T> {
  id?: string;
  email?: string;
  message: string;
  status?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PublicApiService {
  private baseUrl = `${environment.apiUrl}/public`;

  constructor(private http: HttpClient) {}

  submitDemoRequest(data: PublicLeadRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/leads`, data);
  }

  submitContactForm(data: ContactRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/contact`, data);
  }

  subscribeNewsletter(data: NewsletterRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/newsletter`, data);
  }
}
