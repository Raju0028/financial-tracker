import { Component, OnInit, inject, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { Transaction } from '../../../models/transaction';
import { RouterLink } from '@angular/router';
import { GoogleSheetsService } from '../../../services/GoogleSheets.service';
import { OwnedList } from '../../../models/owned-list';
import { Router } from '@angular/router';

@Component({
  imports: [CommonModule],
  standalone: true,
  selector: 'app-dashboard',
  styleUrls: ['./dashboard.css'],
  templateUrl: './dashboard.html',
})


export class Dashboard implements OnInit {
  private readonly apiService = inject(ApiService);
  private readonly googleSheetsService = inject(GoogleSheetsService);
  private readonly router = inject(Router);

  transactions: Transaction[] = [];
  lastOwnedList = signal<OwnedList | null>(null);
  ownedLists: OwnedList[] = [];
  // Expose a simple non-callable property for templates to avoid template type-checker issues
  get lastOwned(): OwnedList | null {
    return this.lastOwnedList();
  }

  totalIncome = 50000;
  totalExpenses = 32500;
  balance = 17500;


  ngOnInit(): void {
    this.loadTransactions();
    this.loadOwnedLists();  
  }

  private loadTransactions(): void {
    this.googleSheetsService.getTransactions().subscribe({
      next: (transactions) => {
        this.transactions = transactions;
        console.log('Transactions loaded:', transactions);
      },
      error: (error) => {
        console.error('Error loading transactions:', error);
      }
    });
  }

  private loadOwnedLists(): void {
    this.googleSheetsService.getOwnedList().subscribe({
      next: (data) => {
        console.log('API data:', data);

        if (data && data.length > 0) {
          this.lastOwnedList.set(data[0]);
          this.ownedLists = data;

          console.log('lastOwnedList assigned:', this.lastOwnedList());
        }
      },
      error: (error) => {
        console.error('Error loading owned list:', error);
      }
    });
  }
  goToOwnedList(): void {
    console.log('Passing to Owned List:', this.ownedLists);

    this.router.navigate(['/owned-list'], {
      state: {
        ownedLists: this.ownedLists
      }
    });
  }
}
