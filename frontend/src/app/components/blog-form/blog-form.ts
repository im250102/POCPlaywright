import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { BLOG_CATEGORIES } from '../../models/blog-post.model';

@Component({
  selector: 'app-blog-form',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './blog-form.html',
  styleUrl: './blog-form.css',
})
export class BlogForm implements OnInit {
  title = '';
  content = '';
  category = 'General';
  categories = BLOG_CATEGORIES;
  error = '';
  isEdit = false;
  postId = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: Api,
  ) {}

  ngOnInit() {
    this.postId = this.route.snapshot.paramMap.get('id') ?? '';
    this.isEdit = this.postId !== '';

    if (this.isEdit) {
      this.api.getPost(this.postId).subscribe({
        next: (post) => {
          this.title = post.title;
          this.content = post.content;
          this.category = post.category;
        },
        error: (err) => console.error('Error loading post:', err),
      });
    }
  }

  save() {
    this.error = '';
    const payload = { title: this.title, content: this.content, category: this.category };
    const operation = this.isEdit
      ? this.api.updatePost(this.postId, payload)
      : this.api.createPost(payload);

    operation.subscribe({
      next: (post) => this.router.navigate(['/blog', post.id]),
      error: (err) => {
        this.error = err.error?.error || 'Error al guardar el post';
      },
    });
  }

  cancel() {
    this.router.navigate(['/blog']);
  }
}