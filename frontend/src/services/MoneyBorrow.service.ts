import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { MoneyBorrow } from '../models/money-borrow';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MoneyBorrowService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;

  getMoneyBorrow(): Observable<MoneyBorrow[]> {
    return this.http.get<MoneyBorrow[]>(
      `${this.apiUrl}/api/moneyborrow`
    );
  }

  addMoneyBorrow(moneyBorrow: MoneyBorrow) {
    return this.http.post(
      `${this.apiUrl}/api/moneyborrow`,
      moneyBorrow
    );
  }

  deleteMoneyBorrow(sheetName: string, rowNumber: number) {
    return this.http.delete(
      `${this.apiUrl}/api/MoneyBorrow`,
      {
        params: {
          sheetName,
          rowNumber
        }
      }
    );
  }
}
