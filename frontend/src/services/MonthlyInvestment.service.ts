import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { MonthlyInvestment } from '../models/monthly-investment';

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
}
