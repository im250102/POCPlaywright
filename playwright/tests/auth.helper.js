const API_BASE = 'http://localhost:5091/api/auth';

const TEST_USER = {
  name: 'Test User',
  email: 'test@playwright.com',
  password: 'TestPass123!',
};

export async function registerTestUser(request) {
  const res = await request.post(`${API_BASE}/register`, {
    data: { ...TEST_USER, confirmPassword: TEST_USER.password },
  });
  return res.ok();
}

export async function loginAndSetup(page) {
  const res = await page.request.post(`${API_BASE}/login`, {
    data: { email: TEST_USER.email, password: TEST_USER.password },
  });
  if (!res.ok()) return false;
  const body = await res.json();
  await page.addInitScript((user) => {
    localStorage.setItem('auth_user', JSON.stringify(user));
    localStorage.setItem('auth_token', user.token);
  }, body);
  return true;
}
