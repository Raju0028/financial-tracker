import { Component, OnInit, inject, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { Transaction } from '../../../models/transaction';
import { RouterLink } from '@angular/router';
import { GoogleSheetsService } from '../../../services/GoogleSheets.service';
import { OwnedList } from '../../../models/owned-list';
import { Router } from '@angular/router';
import { LoanService } from '../../../services/Loan.service';
import { LoanSummary } from '../../../models/loan-summary';
import { DatePipe } from '@angular/common';
import { MonthlyInvestmentService } from '../../../services/MonthlyInvestment.service';
import { MonthlyInvestment } from '../../../models/monthly-investment';

@Component({
  imports: [CommonModule, RouterLink],
  providers: [DatePipe],
  standalone: true,
  selector: 'app-dashboard',
  styleUrls: ['./dashboard.css'],
  templateUrl: './dashboard.html',
})


export class Dashboard implements OnInit {
  private readonly apiService = inject(ApiService);
  private readonly googleSheetsService = inject(GoogleSheetsService);
  private readonly router = inject(Router);
  private readonly loanService = inject(LoanService);
  private readonly monthlyInvestmentService = inject(MonthlyInvestmentService);

  currentDate = signal(
    new Date().toISOString().split('T')[0]
  );
  transactions: Transaction[] = [];
  lastOwnedList = signal<OwnedList | null>(null);
  ownedLists: OwnedList[] = [];
  loanSummary = signal<LoanSummary | null>(null);
  monthlyInvestment = signal<MonthlyInvestment | null>(null);

  // Expose a simple non-callable property for templates to avoid template type-checker issues
  get lastOwned(): OwnedList | null {
    return this.lastOwnedList();
  }

  totalIncome = 50000;

  ngOnInit(): void {
    this.loadTransactions();
    this.loadOwnedLists();
    this.loadLoanSummary();
    this.loadMonthlyInvestment();
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

  private loadLoanSummary(): void {
    this.loanService.getLoanSummary().subscribe({
      next: (data) => {
        this.loanSummary.set(data);
        console.log('Loan summary loaded:', data);
      },
      error: (error) => {
        console.error('Error loading loan summary:', error);
      }
    });
  }

  private loadMonthlyInvestment(): void {
    this.monthlyInvestmentService.getMonthlyInvestment().subscribe({
      next: (data) => {
        this.monthlyInvestment.set(data);
        console.log('Monthly investment loaded:', data);
      },
      error: (error) => {
        console.error('Error loading monthly investment:', error);
      }
    });
  }
}
