import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyInvestmentService } from '../../../services/MonthlyInvestment.service';
import { MonthlyInvestment } from '../../../models/monthly-investment';
import { RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AddMonthlyInvestment } from '../../../models/add-monthly-investment';

@Component({
  selector: 'monthly-investment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './monthly-investment.html',
  styleUrl: './monthly-investment.css'
})
export class MonthlyInvestmentPage implements OnInit,OnDestroy {

  private readonly monthlyInvestmentService =
    inject(MonthlyInvestmentService);

  monthlyInvestment = signal<MonthlyInvestment | null>(null);

  months = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];

  expenses = [
    'Gas Used',
    'Electricity',
    'Mobile',
    'Gift',
    'Car Service',
    'Car Insurance',
    'Bike',
    'CNG',
    'WIFI',
    'Tour',
    'Friends Tour',
    'House Product',
    'Emergency AMT'
  ];

  isAddModalOpen = signal(false);

  selectedMonth = signal('');
  selectedExpense = signal('');

  prices = signal<number[]>([0]);

  private readonly destroy$ = new Subject<void>();


  ngOnInit(): void {
    this.loadMonthlyInvestment();
  }

  private loadMonthlyInvestment(): void {
    this.monthlyInvestmentService.getMonthlyInvestment().subscribe({
      next: (data) => {
        this.monthlyInvestment.set(data);
        console.log('Monthly investment loaded:', data);
      },
      error: (error) => {
        console.error(
          'Error loading monthly investment:',
          error
        );
      }
    });
  }

  private getCurrentMonth(): string {
    return this.months[new Date().getMonth()];
  }

  openAddModal(expense: string): void {
    this.selectedMonth.set(this.getCurrentMonth());
    this.selectedExpense.set(expense);

    this.isAddModalOpen.set(true);

    this.loadExistingPrices();
  }

  closeAddModal(): void {
    this.isAddModalOpen.set(false);
    this.prices.set([0]);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadExistingPrices(): void {
    const month = this.selectedMonth();
    const expense = this.selectedExpense();

    if (!month || !expense) {
      return;
    }

    this.monthlyInvestmentService
      .getExistingPrices(month, expense)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (prices) => {
          this.prices.set(
            prices.length > 0 ? prices : [0]
          );
        },
        error: (error) => {
          console.error(
            'Error loading existing prices:',
            error
          );

          this.prices.set([0]);
        }
      });
  }
  onMonthChange(month: string): void {
    this.selectedMonth.set(month);
    this.loadExistingPrices();
  }
  onExpenseChange(expense: string): void {
    this.selectedExpense.set(expense);
    this.loadExistingPrices();
  }
  addPriceField(): void {
    this.prices.update(prices => [
      ...prices,
      0
    ]);
  }
  removePriceField(index: number): void {
    this.prices.update(prices =>
      prices.filter((_, i) => i !== index)
    );
  }
  updatePrice(
    index: number,
    value: string
  ): void {
    const price = Number(value);

    this.prices.update(prices => {
      const updated = [...prices];
      updated[index] = Number.isFinite(price)
        ? price
        : 0;

      return updated;
    });
  }

  saveMonthlyInvestment(): void {
    const request: AddMonthlyInvestment = {
      month: this.selectedMonth(),
      expense: this.selectedExpense(),
      prices: this.prices().filter(price => price > 0)
    };

    if (!request.month || !request.expense) {
      alert('Please select month and expense.');
      return;
    }

    if (request.prices.length === 0) {
      alert('Please enter at least one price.');
      return;
    }

    this.monthlyInvestmentService
      .addMonthlyInvestment(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          alert('Monthly investment added successfully.');

          this.closeAddModal();

          // Refresh the displayed values
          this.loadMonthlyInvestment();
        },
        error: (error) => {
          console.error(
            'Error adding monthly investment:',
            error
          );

          alert('Failed to add monthly investment.');
        }
      });
  }
}
