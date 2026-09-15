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
  currentUserId = '';

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.currentUserId = this.getCurrentUserId();
    this.loadUsers();
  }

  private getCurrentUserId(): string {
    try {
      const stored = localStorage.getItem('auth_user');
      return stored ? (JSON.parse(stored).id ?? '') : '';
    } catch {
      return '';
    }
  }

  isSelf(user: UserItem): boolean {
    return user.id === this.currentUserId;
  }

  isLastAdmin(user: UserItem): boolean {
    return user.role === 'Admin' && this.users.filter((u) => u.role === 'Admin').length <= 1;
  }

  canDelete(user: UserItem): boolean {
    return !this.isSelf(user) && !this.isLastAdmin(user);
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

  deleteUser(user: UserItem) {
    if (!window.confirm(`¿Seguro que quieres eliminar a ${user.name}?`)) return;

    this.api.deleteUser(user.id).subscribe({
      next: () => {
        this.users = this.users.filter((u) => u.id !== user.id);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting user:', err),
    });
  }
}
