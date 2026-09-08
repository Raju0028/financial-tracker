import { Component } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-back-button',
  standalone: true,
  templateUrl: './back-button.html',
  styleUrl: './back-button.css'
})
export class BackButton {
  constructor(private readonly location: Location) { }

  goBack(): void {
    this.location.back();
  }
}
