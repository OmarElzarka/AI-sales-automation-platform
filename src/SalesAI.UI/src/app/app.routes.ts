import { Routes } from '@angular/router';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { LeadListComponent } from './features/leads/lead-list/lead-list.component';
import { LeadDetailComponent } from './features/leads/lead-detail/lead-detail.component';
import { DealBoardComponent } from './features/deals/deal-board/deal-board.component';
import { TaskListComponent } from './features/tasks/task-list/task-list.component';
import { EmailComposerComponent } from './features/outreach/email-composer/email-composer.component';
import { ReportsComponent } from './features/reports/reports.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'leads', component: LeadListComponent },
      { path: 'leads/:id', component: LeadDetailComponent },
      { path: 'deals', component: DealBoardComponent },
      { path: 'tasks', component: TaskListComponent },
      { path: 'outreach', component: EmailComposerComponent },
      { path: 'reports', component: ReportsComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
