import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { UserDetail } from '../models/user-detail';
import { Transaction } from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;

  getUsers(): Observable<UserDetail[]> {
    return this.http.get<UserDetail[]>(
      `${this.apiUrl}/api/user`
    );
  }
}
