import { Component, signal } from '@angular/core';
import { Header } from './components/header/header';
import { Footer } from './components/footer/footer';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './components/loading/loading';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer, LoadingComponent],
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('financial-trackerUI');
}
