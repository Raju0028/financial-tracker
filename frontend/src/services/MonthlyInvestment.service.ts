import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { MonthlyInvestment } from '../models/monthly-investment';
import { AddMonthlyInvestment } from '../models/add-monthly-investment';

@Injectable({
  providedIn: 'root'
})
export class MonthlyInvestmentService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;

  getMonthlyInvestment() {
    return this.http.get<MonthlyInvestment>(
      `${this.apiUrl}/api/MonthlyInvestment`
    );
  }

  getExistingPrices(month: string, expense: string) {
    return this.http.get<number[]>(
      `${this.apiUrl}/api/MonthlyInvestment/prices`,
      {
        params: {
          month,
          expense
        }
      }
    );
  }

  addMonthlyInvestment(request: AddMonthlyInvestment) {
    return this.http.post(
      `${this.apiUrl}/api/MonthlyInvestment`,
      request
    );
  }
}
