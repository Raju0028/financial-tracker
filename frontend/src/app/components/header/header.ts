import { Component, OnInit, inject, signal, } from '@angular/core';
import { ApiService } from '../../../services/api.service';
import { UserDetail } from '../../../models/user-detail';

@Component({
  imports: [],
  selector: 'app-header',
  styleUrl: './header.css',
  templateUrl: './header.html',
})
export class Header {
  private readonly apiService = inject(ApiService);
  users = signal<UserDetail | null>(null);


  ngOnInit(): void {
    this.loadUsers();
  }
  private loadUsers(): void {
    this.apiService.getUsers().subscribe({
      next: (users: UserDetail[]) => {
        this.users.set(users[0] || null);

        console.log('Users loaded:', users);
      },
      error: (error) => {
        console.error('Error loading users:', error);
      }
    });
  }
}
