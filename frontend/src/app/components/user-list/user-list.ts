import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { UserItem, UserAccessItem } from '../../models/user.model';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css',
})
export class UserList implements OnInit {
  users: UserItem[] = [];
  selectedUser: UserItem | null = null;
  accesses: UserAccessItem[] = [];

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.api.getUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading users:', err),
    });
  }

  showAccesses(user: UserItem) {
    this.selectedUser = user;
    this.api.getUserAccesses(user.id).subscribe({
      next: (data) => {
        this.accesses = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading accesses:', err),
    });
  }

  changeRole(user: UserItem, role: string) {
    this.api.updateUserRole(user.id, role).subscribe({
      next: () => {
        user.role = role;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error updating role:', err),
    });
  }

  onRoleChange(user: UserItem, event: Event) {
    const role = (event.target as HTMLSelectElement).value;
    this.changeRole(user, role);
  }
}
