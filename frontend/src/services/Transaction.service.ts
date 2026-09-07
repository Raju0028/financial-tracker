import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { Transaction } from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;


  getRecentTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(
      `${this.apiUrl}/api/transactions/recent`
    );
  }

  addRecentTransaction(transaction: Transaction) {
    return this.http.post(
      `${this.apiUrl}/api/transactions/recent`,
      transaction
    );
  }
}
