import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
    });
    service = TestBed.inject(AuthService);
  });

  it('should consider a stored token as an active session', () => {
    localStorage.setItem('auth_token', 'sample-token');

    const freshService = TestBed.inject(AuthService);

    expect(freshService.isLoggedIn).toBeTrue();
  });
});
