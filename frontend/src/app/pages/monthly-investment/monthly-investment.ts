import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyInvestmentService } from '../../../services/MonthlyInvestment.service';
import { MonthlyInvestment } from '../../../models/monthly-investment';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'monthly-investment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './monthly-investment.html',
  styleUrl: './monthly-investment.css'
})
export class MonthlyInvestmentPage implements OnInit {

  private readonly monthlyInvestmentService =
    inject(MonthlyInvestmentService);

  monthlyInvestment = signal<MonthlyInvestment | null>(null);

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
}
