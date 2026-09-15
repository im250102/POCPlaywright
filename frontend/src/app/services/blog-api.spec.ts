import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Api } from './api';
import { environment } from '../../environments/environment';

describe('Api - métodos del blog', () => {
  let service: Api;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(Api);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should list posts without category filter', () => {
    service.getPosts().subscribe((posts) => {
      expect(posts).toEqual([]);
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/blog`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should list posts filtering by category', () => {
    service.getPosts('Tecnologia').subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/blog?category=Tecnologia`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should fetch a single post', () => {
    service.getPost('abc').subscribe((post) => {
      expect(post.id).toBe('abc');
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/blog/abc`);
    expect(req.request.method).toBe('GET');
    req.flush({ id: 'abc', comments: [] });
  });

  it('should create a post via POST', () => {
    const input = { title: 'T', content: 'C', category: 'General' };
    service.createPost(input).subscribe((post) => {
      expect(post.id).toBe('new-id');
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/blog`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(input);
    req.flush({ id: 'new-id' });
  });

  it('should update a post via PUT', () => {
    const input = { title: 'T2', content: 'C2', category: 'Personal' };
    service.updatePost('abc', input).subscribe((post) => {
      expect(post.id).toBe('abc');
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/blog/abc`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(input);
    req.flush({ id: 'abc' });
  });

  it('should delete a post via DELETE', () => {
    service.deletePost('abc').subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/blog/abc`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should add a comment via POST to /blog/:id/comments', () => {
    service.addComment('abc', 'Me gusta').subscribe((comment) => {
      expect(comment.postId).toBe('abc');
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/blog/abc/comments`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ text: 'Me gusta' });
    req.flush({ id: 'c1', postId: 'abc' });
  });
});