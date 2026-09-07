import { Component, inject } from '@angular/core';
import { LoadingService } from '../../../services/loading.service';

@Component({
  selector: 'app-loading',
  standalone: true,
  templateUrl: './loading.html',
  styleUrl: './loading.css'
})
export class LoadingComponent {

  readonly loadingService = inject(LoadingService);

}
