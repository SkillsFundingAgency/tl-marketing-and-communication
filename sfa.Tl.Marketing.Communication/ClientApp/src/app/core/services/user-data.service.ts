import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';

// interface RouteDataService {
//   pageTitle: string;
// }
// const initialRegistration: RouteDataService = {
//   // Example initial dummy data
//   pageTitle: 'home',
// };

@Injectable({
  providedIn: 'root',
})
export class DataService {
  // Observable registration source
  private routingDataSource$ = new BehaviorSubject<string>('');
  public routingData$: Observable<string> = this.routingDataSource$.asObservable();

  constructor() {}

  updateState(data) {
    this.routingDataSource$.next(data);
  }
}
