import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup } from './auth.helper.js';

const AUTH_BASE = 'http://localhost:5091/api/auth';
const API_BASE = 'http://localhost:5091/api';

async function createUser(request, name, email) {
  const password = 'TestPass123!';
  const res = await request.post(`${AUTH_BASE}/register`, {
    data: { name, email, password, confirmPassword: password },
  });
  return { res, password };
}

async function login(request, email, password) {
  const res = await request.post(`${AUTH_BASE}/login`, {
    data: { email, password },
  });
  if (!res.ok()) return { res, body: null };
  return { res, body: await res.json() };
}

test.describe('Administración de Usuarios', () => {
  test.beforeAll(async ({ request }) => {
    await registerTestUser(request);
  });

  test('Escenario 1: El primer usuario registrado es Director de la clínica', async ({ request }) => {
    const { body } = await login(request, 'ignacio@medico.es', 'TestPass123!');
    expect(body).not.toBeNull();
    expect(body.role).toBe('Admin');
  });

  test('Escenario 2: El Director ve el enlace de administración de usuarios', async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/');
    await expect(page.locator('nav a', { hasText: 'Administración usuarios' })).toBeVisible();
  });

  test('Escenario 3: El Médico no ve el enlace de administración', async ({ request, page }) => {
    const unique = Date.now();
    await createUser(request, `Médico ${unique}`, `medico${unique}@test.com`);
    const { body } = await login(request, `medico${unique}@test.com`, 'TestPass123!');
    expect(body).not.toBeNull();
    await page.addInitScript((user) => {
      localStorage.setItem('auth_user', JSON.stringify(user));
      localStorage.setItem('auth_token', user.token);
    }, body);
    await page.goto('/');
    await expect(page.locator('nav a', { hasText: 'Administración usuarios' })).toHaveCount(0);
  });

  test('Escenario 4: Visualizar el listado de usuarios', async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/users');
    await expect(page.locator('h2')).toHaveText('Administración usuarios');
    await expect(page.locator('table thead th')).toHaveText([
      'Nombre',
      'Email',
      'Rol',
      'Fecha de registro',
      'Último acceso',
      'Acciones',
    ]);
    await expect(page.locator('tbody tr', { hasText: 'ignacio@medico.es' })).toBeVisible();
  });

  test('Escenario 5: Visualizar los accesos a la web de un usuario', async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/users');
    await page.locator('tbody tr', { hasText: 'ignacio@medico.es' }).locator('button', { hasText: 'Ver accesos' }).click();
    await expect(page.locator('table th', { hasText: 'Fecha de acceso' })).toBeVisible();
    await expect(page.locator('tbody tr td.latest-access')).not.toHaveCount(0);
  });

  test('Escenario 6: El Médico no puede acceder a la administración', async ({ request, page }) => {
    const unique = Date.now();
    await createUser(request, `Médico Acceso ${unique}`, `medicoacceso${unique}@test.com`);
    const { res, body } = await login(request, `medicoacceso${unique}@test.com`, 'TestPass123!');
    expect(res.ok()).toBeTruthy();
    const token = body.token;

    const usersResponse = await request.get(`${API_BASE}/users`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    expect(usersResponse.status()).toBe(403);

    await page.addInitScript((user) => {
      localStorage.setItem('auth_user', JSON.stringify(user));
      localStorage.setItem('auth_token', user.token);
    }, body);
    await page.goto('/users');
    await expect(page).not.toHaveURL('/users');
  });

  test('Escenario 7: El Director cambia el rol de un usuario', async ({ request }) => {
    const unique = Date.now();
    await createUser(request, `Médico Cambio ${unique}`, `medicocambio${unique}@test.com`);
    const admin = await login(request, 'ignacio@medico.es', 'TestPass123!');
    const adminToken = admin.body.token;

    const usersResponse = await request.get(`${API_BASE}/users`, {
      headers: { Authorization: `Bearer ${adminToken}` },
    });
    const users = await usersResponse.json();
    const target = users.find((u) => u.email === `medicocambio${unique}@test.com`);
    expect(target).toBeTruthy();

    const changeResponse = await request.put(`${API_BASE}/users/${target.id}/role`, {
      headers: { Authorization: `Bearer ${adminToken}` },
      data: { role: 'Admin' },
    });
    expect(changeResponse.status()).toBe(204);

    const updatedUsers = await (await request.get(`${API_BASE}/users`, {
      headers: { Authorization: `Bearer ${adminToken}` },
    })).json();
    const updated = updatedUsers.find((u) => u.id === target.id);
    expect(updated.role).toBe('Admin');
  });
});