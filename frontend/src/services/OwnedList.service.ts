import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { OwnedList } from '../models/owned-list';

@Injectable({
  providedIn: 'root'
})
export class OwnedListService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl;

  getOwnedList(): Observable<OwnedList[]> {
    return this.http.get<OwnedList[]>(
      `${this.apiUrl}/api/ownedList/ownerlists`
    );
  }

  addOwnedList(ownerList: OwnedList) {
    return this.http.post(
      `${this.apiUrl}/api/ownedList/ownerlists`,
      ownerList
    );
  }

  updateOwnedList(
    rowNumber: number,
    ownerList: OwnedList
  ) {
    return this.http.put(
      `${this.apiUrl}/api/ownedList/ownerlists/${rowNumber}`,
      ownerList
    );
  }

  deleteOwnedList(
    rowNumber: number
  ) {
    return this.http.delete(
      `${this.apiUrl}/api/ownedList/ownerlists/${rowNumber}`
    );
  }
}
