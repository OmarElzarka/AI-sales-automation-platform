import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ContactComponent } from './pages/contact/contact.component';
import { BookDemoComponent } from './pages/book-demo/book-demo.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'book-demo', component: BookDemoComponent },
  { path: '**', redirectTo: '' }
];
