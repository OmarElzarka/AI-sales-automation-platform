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
  error: string | null = null;
  activeTab: 'pending' | 'completed' = 'pending';

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.loading = true;
    this.error = null;
    this.apiService.get('/tasks').subscribe({
      next: (data: any) => {
        this.tasks = data.items || data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load tasks:', err);
        this.error = 'Failed to load tasks.';
        this.tasks = [];
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
    const newStatus = !task.isCompleted;
    this.apiService.put(`/tasks/${task.id}/status`, { status: newStatus ? 'Completed' : 'Pending' }).subscribe({
      next: () => {
        task.isCompleted = newStatus;
      },
      error: (err) => {
        console.error('Failed to update task status:', err);
        alert('Failed to update task status.');
      }
    });
  }
}
