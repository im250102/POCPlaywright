import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { AuthService } from '../../services/auth.service';
import { BlogPost, BLOG_CATEGORIES } from '../../models/blog-post.model';

@Component({
  selector: 'app-blog-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './blog-list.html',
  styleUrl: './blog-list.css',
})
export class BlogList implements OnInit {
  posts: BlogPost[] = [];
  categories = BLOG_CATEGORIES;
  selectedCategory = '';

  constructor(
    private api: Api,
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getPosts(this.selectedCategory || undefined).subscribe({
      next: (data) => {
        this.posts = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading posts:', err),
    });
  }

  filter(category: string) {
    this.selectedCategory = category;
    this.load();
  }
}