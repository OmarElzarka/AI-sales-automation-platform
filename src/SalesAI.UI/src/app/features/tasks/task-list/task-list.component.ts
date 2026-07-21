import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

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
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  tasks: SalesTask[] = [];
  loading = true;
  error: string | null = null;
  activeTab: 'pending' | 'completed' = 'pending';

  showNewTaskModal = false;
  submitting = false;
  newTask = {
    title: '',
    description: '',
    type: 'Todo',
    priority: 'Medium',
    dueDate: '',
    assignedToId: ''
  };

  constructor(private apiService: ApiService, private authService: AuthService) {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      const user = JSON.parse(userStr);
      this.newTask.assignedToId = user.id || '00000000-0000-0000-0000-000000000001';
    } else {
      this.newTask.assignedToId = '00000000-0000-0000-0000-000000000001';
    }
  }

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

  openNewTaskModal() {
    this.showNewTaskModal = true;
    this.newTask.title = '';
    this.newTask.description = '';
    
    // Set due date to today by default
    const today = new Date();
    this.newTask.dueDate = today.toISOString().split('T')[0];
  }

  closeNewTaskModal() {
    this.showNewTaskModal = false;
  }

  submitNewTask() {
    if (!this.newTask.title || !this.newTask.dueDate) {
      alert('Title and Due Date are required.');
      return;
    }
    
    this.submitting = true;
    this.apiService.post('/tasks', this.newTask).subscribe({
      next: () => {
        this.submitting = false;
        this.showNewTaskModal = false;
        this.loadTasks();
      },
      error: (err) => {
        console.error('Failed to create task:', err);
        alert('Failed to create task.');
        this.submitting = false;
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
