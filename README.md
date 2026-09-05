# financial-tracker
The Repository used to track our daily expenses for single personal use.

# UI structure
src/app/
│
├── components/
│   ├── header/
│   └── footer/
│
├── pages/
│   ├── login/
│   ├── dashboard/
│   ├── transactions/
│   ├── expenses/
│   ├── income/
│   └── reports/
│
├── services/
│   └── ...
│
├── models/
│   └── ...
│
├── app.ts
├── app.html
├── app.css
└── app.routes.ts

# dashboard
┌─────────────────────────────────────────────────────────────┐
│ Financial Tracker                         Profile            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ Welcome back! 👋                                            │
│ Here's your financial overview                              │
│                                                             │
│ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐          │
│ │ Total Income │ │ Total Expense│ │ Balance      │          │
│ │ ₹50,000      │ │ ₹32,500      │ │ ₹17,500      │          │
│ └──────────────┘ └──────────────┘ └──────────────┘          │
│                                                             │
│ ┌─────────────────────────┐ ┌─────────────────────────────┐ │
│ │ Expense Overview        │ │ Recent Transactions         │ │
│ │                         │ │                             │ │
│ │       Chart             │ │ Food       - ₹500           │ │
│ │                         │ │ Salary   + ₹50,000           │ │
│ │                         │ │ Petrol     - ₹2,000          │ │
│ └─────────────────────────┘ └─────────────────────────────┘ │
│                                                             │
└─────────────────────────────────────────────────────────────┘