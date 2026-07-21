import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';

interface SalesTask {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  type: string; // 'Call', 'Email', 'Meeting', 'Todo'
  relatedEntity: string; // e.g., 'Lead: John Doe'
}

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  tasks: SalesTask[] = [];
  loading = true;
  activeTab: 'pending' | 'completed' = 'pending';

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.apiService.get('/tasks').subscribe({
      next: (data: any) => {
        this.tasks = data;
        this.loading = false;
      },
      error: () => {
        const today = new Date();
        const tomorrow = new Date(today);
        tomorrow.setDate(tomorrow.getDate() + 1);

        this.tasks = [
          { id: '1', title: 'Follow up on proposal', description: 'Check if they reviewed the pricing', dueDate: today.toISOString(), isCompleted: false, type: 'Call', relatedEntity: 'Deal: Enterprise License' },
          { id: '2', title: 'Send outreach email', description: 'Use AI playbook template', dueDate: today.toISOString(), isCompleted: false, type: 'Email', relatedEntity: 'Lead: Jane Smith' },
          { id: '3', title: 'Product Demo', description: 'Show the new reporting features', dueDate: tomorrow.toISOString(), isCompleted: false, type: 'Meeting', relatedEntity: 'Lead: Bob Johnson' },
          { id: '4', title: 'Update CRM records', description: 'Clean up stale leads', dueDate: today.toISOString(), isCompleted: true, type: 'Todo', relatedEntity: 'General' }
        ];
        this.loading = false;
      }
    });
  }

  getPendingTasks() {
    return this.tasks.filter(t => !t.isCompleted);
  }

  getCompletedTasks() {
    return this.tasks.filter(t => t.isCompleted);
  }

  getIconForType(type: string) {
    switch(type) {
      case 'Call': return 'phone';
      case 'Email': return 'email';
      case 'Meeting': return 'event';
      default: return 'check_circle_outline';
    }
  }

  toggleTaskCompletion(task: SalesTask) {
    task.isCompleted = !task.isCompleted;
    // this.apiService.put(`/tasks/${task.id}`, task).subscribe();
  }
}
