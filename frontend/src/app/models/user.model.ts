export interface UserItem {
  id: string;
  name: string;
  email: string;
  role: string;
  createdAt: string;
  lastLoginAt?: string | null;
}

export interface UserAccessItem {
  id: string;
  userId: string;
  loginAt: string;
}
