import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { LoanService } from '../../../services/Loan.service';
import { LoanSummary } from '../../../models/loan-summary';
import { Loan as LoanModel } from '../../../models/loan';
import { LoanRepayment } from '../../../models/loan-repayment';
import { RouterLink } from '@angular/router';


@Component({
  selector: 'loan',
  standalone: true,
  imports: [CommonModule, RouterLink, NgIf],
  templateUrl: './loan.html',
  styleUrl: './loan.css'
})
export class Loan implements OnInit {

  private readonly loanService = inject(LoanService);

  loanSummary = signal<LoanSummary | null>(null);

  loans = signal<LoanModel[]>([]);
  repayments = signal<LoanRepayment[]>([]);

  selectedYear = signal(new Date().getFullYear());

  showLoanModal = signal(false);

  loanForm: LoanModel = {
    rowNumber: 0,
    date: '',
    loanAmount: 0,
    duration: '',
    from: '',
    totalLoan: 0,
    status: 'Active'
  };

  showRepaymentModal = signal(false);

  repaymentForm: LoanRepayment = {
    rowNumber: 0,
    date: '',
    repaymentAmount: 0,
    to: ''
  };

  filteredLoans = computed(() =>
    this.loans().filter(loan =>
      this.getYear(loan.date) === this.selectedYear()
    )
  );

  filteredRepayments = computed(() =>
    this.repayments().filter(repayment =>
      this.getYear(repayment.date) === this.selectedYear()
    )
  );

  ngOnInit(): void {
    this.loadLoanSummary();
    this.loadLoans();
    this.loadRepayments();
  }

  private loadLoanSummary(): void {
    this.loanService.getLoanSummary().subscribe({
      next: (data) => {
        this.loanSummary.set(data);
      },
      error: (error) => {
        console.error('Error loading loan summary:', error);
      }
    });
  }

  private loadLoans(): void {
    this.loanService.getLoans().subscribe({
      next: (data) => {
        this.loans.set(data);
      },
      error: (error) => {
        console.error('Error loading loans:', error);
      }
    });
  }

  private loadRepayments(): void {
    this.loanService.getLoanRepayments().subscribe({
      next: (data) => {
        this.repayments.set(data);
        console.log("loading repayments", data)
      },
      error: (error) => {
        console.error('Error loading repayments:', error);
      }
    });
  }

  changeYear(year: number): void {
    this.selectedYear.set(year);
  }

  private getYear(dateValue: string): number | null {
    const date = new Date(dateValue);

    if (isNaN(date.getTime())) {
      return null;
    }

    return date.getFullYear();
  }

  openLoanModal(): void {
    this.loanForm = {
      rowNumber: 0,
      date: new Date().toISOString().split('T')[0],
      loanAmount: 0,
      duration: '',
      from: '',
      totalLoan: 0,
      status: 'Active'
    };

    this.showLoanModal.set(true);
  }

  closeLoanModal(): void {
    this.showLoanModal.set(false);
  }

  saveLoan(): void {
    if (
      !this.loanForm.date ||
      this.loanForm.loanAmount <= 0 ||
      !this.loanForm.from
    ) {
      alert('Please fill in all required fields.');
      return;
    }

    this.loanService.addLoan(this.loanForm).subscribe({
      next: () => {
        this.closeLoanModal();

        this.loadLoans();
        this.loadLoanSummary();

        alert('Loan added successfully.');
      },
      error: (error) => {
        console.error('Error adding loan:', error);
        alert('Failed to add loan.');
      }
    });
  }

  openRepaymentModal(): void {
    this.repaymentForm = {
      rowNumber: 0,
      date: new Date().toISOString().split('T')[0],
      repaymentAmount: 0,
      to: ''
    };

    this.showRepaymentModal.set(true);
  }

  closeRepaymentModal(): void {
    this.showRepaymentModal.set(false);
  }

  saveRepayment(): void {
    if (
      !this.repaymentForm.date ||
      this.repaymentForm.repaymentAmount <= 0 ||
      !this.repaymentForm.to
    ) {
      alert('Please fill in all required fields.');
      return;
    }

    this.loanService.addLoanRepayment(this.repaymentForm).subscribe({
      next: () => {
        this.closeRepaymentModal();

        this.loadRepayments();
        this.loadLoanSummary();

        alert('Repayment added successfully.');
      },
      error: (error) => {
        console.error('Error adding repayment:', error);
        alert('Failed to add repayment.');
      }
    });
  }
}
