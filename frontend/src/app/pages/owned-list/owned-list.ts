import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OwnedList as OwnedListModel } from '../../../models/owned-list';
import { DecimalPipe, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GoogleSheetsService } from '../../../services/GoogleSheets.service';

@Component({
  selector: 'app-owned-list',
  standalone: true,
  imports: [
    RouterLink,
    DecimalPipe,
    FormsModule,
    NgIf
  ],
  styleUrl: './owned-list.css',
  templateUrl: './owned-list.html',
})
export class OwnedList {

  private readonly googleSheetsService =
    inject(GoogleSheetsService);

  // Modal
  showAddModal = signal(false);

  // Edit mode
  isEditMode = signal(false);

  // Currently editing row
  editingRowNumber = signal<number | null>(null);

  // Form model
  newOwnedList: OwnedListModel = {
    date: '',
    item: '',
    cost: 0,
    amount: '',
    status: '',
    comments: ''
  };

  // Owned list
  ownedLists = signal<OwnedListModel[]>([]);


  constructor() {
    this.loadOwnedLists();
  }


  // =====================================================
  // LOAD OWNED LIST
  // =====================================================

  private loadOwnedLists(): void {
    this.googleSheetsService
      .getOwnedList()
      .subscribe({
        next: (data) => {
          this.ownedLists.set(data);
        },
        error: (error) => {
          console.error( 'Error loading owned lists:', error );
        }
      });
  }

  // =====================================================
  // OPEN ADD MODAL
  // =====================================================

  openAddModal(): void {
    this.newOwnedList =
      this.createEmptyOwnedItem();
    this.isEditMode.set(false);
    this.editingRowNumber.set(null);
    this.showAddModal.set(true);
  }

  // =====================================================
  // OPEN EDIT MODAL
  // =====================================================

  openEditModal( ownedList: OwnedListModel ): void {
    // Create a copy so the table is not modified
    // before the user clicks Update.
    this.newOwnedList = {
      ...ownedList
    };
    this.isEditMode.set(true);
    this.editingRowNumber.set(
      ownedList.rowNumber ?? null
    );
    this.showAddModal.set(true);
  }

  // =====================================================
  // CLOSE MODAL
  // =====================================================

  closeAddModal(): void {
    this.showAddModal.set(false);
    this.isEditMode.set(false);
    this.editingRowNumber.set(null);
  }

  // =====================================================
  // SAVE / UPDATE
  // =====================================================

  saveOwnedList(): void {
    if (this.isEditMode()) {
      const rowNumber =  this.editingRowNumber();

      if (!rowNumber) {
        return;
      }
      this.googleSheetsService.updateOwnedList(rowNumber,this.newOwnedList)
        .subscribe({
          next: (response) => {
            this.closeAddModal();
            this.loadOwnedLists();
          },
          error: (error) => {
            console.error('Error updating owned list:',error);
          }
        });
      return;
    }

    this.googleSheetsService.addOwnedList(this.newOwnedList)
      .subscribe({
        next: (response) => {
          this.closeAddModal();
          this.loadOwnedLists();
        },
        error: (error) => {
          console.error( 'Error adding owned list:',error );
        }
      });
  }

  // =====================================================
  // DELETE
  // =====================================================

  deleteOwnedList( ownedList: OwnedListModel ): void {
    const rowNumber = ownedList.rowNumber;
    if (!rowNumber) {
      return;
    }
    const confirmed = window.confirm(`Are you sure you want to delete "${ownedList.item}"?`);
    if (!confirmed) {
      return;
    }
    this.googleSheetsService .deleteOwnedList(rowNumber)
      .subscribe({
        next: (response) => {
          this.loadOwnedLists();
        },
        error: (error) => {
          console.error('Error deleting owned list:', error);
        }
      });
  }

  // =====================================================
  // EMPTY MODEL
  // =====================================================
  private createEmptyOwnedItem():
    OwnedListModel {
    return {
      date: '',
      item: '',
      cost: 0,
      amount: '',
      status: '',
      comments: ''
    };
  }
}
