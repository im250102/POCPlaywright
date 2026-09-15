import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { AuthService } from '../../services/auth.service';
import { BlogPostDetail } from '../../models/blog-post.model';

@Component({
  selector: 'app-blog-post',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './blog-post.html',
  styleUrl: './blog-post.css',
})
export class BlogPostView implements OnInit {
  post?: BlogPostDetail;
  commentText = '';
  commentError = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: Api,
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  get isAuthor(): boolean {
    return !!this.post && this.auth.user?.id === this.post.userId;
  }

  get isAdmin(): boolean {
    return this.auth.user?.role === 'Admin';
  }

  get canModerate(): boolean {
    return !!this.post && (this.isAuthor || this.isAdmin);
  }

  ngOnInit() {
    this.load();
  }

  private load() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/blog']);
      return;
    }

    this.api.getPost(id).subscribe({
      next: (data) => {
        this.post = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading post:', err);
        this.router.navigate(['/blog']);
      },
    });
  }

  addComment() {
    if (!this.post || !this.commentText.trim()) {
      return;
    }

    this.commentError = '';
    this.api.addComment(this.post.id, this.commentText.trim()).subscribe({
      next: () => {
        this.commentText = '';
        this.load();
      },
      error: (err) => {
        this.commentError = err.error?.error || 'Error al enviar el comentario';
        this.cdr.detectChanges();
      },
    });
  }

  deletePost() {
    if (!this.post) {
      return;
    }

    this.api.deletePost(this.post.id).subscribe({
      next: () => this.router.navigate(['/blog']),
      error: (err) => {
        this.commentError = err.error?.error || 'Error al eliminar el post';
        this.cdr.detectChanges();
      },
    });
  }
}