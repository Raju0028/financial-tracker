import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Loan } from './pages/loan/loan';
import { OwnedList } from './pages/owned-list/owned-list';
import { MonthlyInvestmentPage } from './pages/monthly-investment/monthly-investment';


export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    component: Dashboard
  },
  {
    path: 'loan',
    component: Loan
  },
  {
    path: 'owned-list',
    component: OwnedList
  },
  {
    path: 'monthly-investment',
    component: MonthlyInvestmentPage
  }

];
