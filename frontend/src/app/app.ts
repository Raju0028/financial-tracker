import { Component, OnInit, inject, signal } from '@angular/core';
import { ApiService } from '../services/api.service';
import { UserDetail } from '../models/user-detail';

@Component({
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})

export class App implements OnInit {
  protected readonly title = signal('financial-trackerUI');

  private readonly apiService = inject(ApiService);

  users: UserDetail[] = [];

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.apiService.getUsers().subscribe({
      next: (users: UserDetail[]) => {
        this.users = users;
        console.log('Users loaded:', users);
      },
      error: (error) => {
        console.error('Error loading users:', error);
      }
    });
  }
}
