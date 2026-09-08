import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Expenses } from './pages/expenses/expenses';
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
    path: 'expenses',
    component: Expenses
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
