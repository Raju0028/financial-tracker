import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { Transaction } from '../models/transaction';
import { OwnedList } from '../models/owned-list';

@Injectable({
  providedIn: 'root'
})
export class GoogleSheetsService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;


  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(
      `${this.apiUrl}/api/transactions`
    );
  }

  getOwnedList(): Observable<OwnedList[]> {
    return this.http.get<OwnedList[]>(
      `${this.apiUrl}/api/transactions/ownerlists`
    );
  }

  addOwnedList(ownerList: OwnedList) {
    return this.http.post(
      `${this.apiUrl}/api/transactions/ownerlists`,
      ownerList
    );
  }

  updateOwnedList(
    rowNumber: number,
    ownerList: OwnedList
  ) {
    return this.http.put(
      `${this.apiUrl}/api/Transactions/ownerlists/${rowNumber}`,
      ownerList
    );
  }

  deleteOwnedList(
    rowNumber: number
  ) {
    return this.http.delete(
      `${this.apiUrl}/api/Transactions/ownerlists/${rowNumber}`
    );
  }
}
