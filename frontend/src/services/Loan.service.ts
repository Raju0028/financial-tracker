import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { LoanSummary } from '../models/loan-summary';
import { Loan } from '../models/loan';
import { LoanRepayment } from '../models/loan-repayment';


@Injectable({
  providedIn: 'root'
})
export class LoanService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;

  getLoanSummary() {
    return this.http.get<LoanSummary>(
      `${this.apiUrl}/api/Loan/summary`
    );
  }

  getLoans() {
    return this.http.get<Loan[]>(
      `${this.apiUrl}/api/Loan`
    );
  }

  getLoanRepayments() {
    return this.http.get<LoanRepayment[]>(
      `${this.apiUrl}/api/Loan/repayments`
    );
  }

  addLoan(loan: Loan) {
    return this.http.post(
      `${this.apiUrl}/api/Loan`,
      loan
    );
  }

  addLoanRepayment(repayment: LoanRepayment) {
    return this.http.post(
      `${this.apiUrl}/api/Loan/repayments`,
      repayment
    );
  }
}
