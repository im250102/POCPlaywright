export interface BlogPost {
  id: string;
  userId: string;
  authorName: string;
  title: string;
  content: string;
  category: string;
  createdAt: string;
  updatedAt?: string;
}

export interface BlogComment {
  id: string;
  postId: string;
  userId: string;
  authorName: string;
  text: string;
  createdAt: string;
}

export interface BlogPostDetail extends BlogPost {
  comments: BlogComment[];
}

export interface BlogPostInput {
  title: string;
  content: string;
  category: string;
}

export const BLOG_CATEGORIES: string[] = ['General', 'Tecnologia', 'Personal', 'Tutorial'];