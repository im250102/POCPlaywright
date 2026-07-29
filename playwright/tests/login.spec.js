import { test, expect } from '@playwright/test';

test.describe('Login', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
  });

  test('debe mostrar el formulario de login', async ({ page }) => {
    await expect(page.locator('h1')).toHaveText('Iniciar Sesión');
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toHaveText('Iniciar Sesión');
  });

  test('el boton de submit debe estar deshabilitado al inicio', async ({ page }) => {
    await expect(page.locator('button[type="submit"]')).toBeDisabled();
  });

  test('debe mostrar error con credenciales invalidas', async ({ page }) => {
    await page.fill('input[type="email"]', 'noexiste@test.com');
    await page.fill('input[type="password"]', 'wrong');
    await page.waitForSelector('button[type="submit"]:not([disabled])');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(3000);
    await expect(page).toHaveURL('/login');
  });

  test('debe permitir navegar al registro', async ({ page }) => {
    await page.click('a[routerLink="/register"]');
    await expect(page).toHaveURL('/register');
    await expect(page.locator('h1')).toHaveText('Crear Cuenta');
  });

  test('debe redirigir al login si no esta autenticado', async ({ page }) => {
    await page.goto('/appointments');
    await expect(page).toHaveURL('/login');
  });

  test('debe iniciar sesion correctamente', async ({ page, request }) => {
    const { registerTestUser, loginAndSetup } = await import('./auth.helper.js');
    await registerTestUser(request);
    await page.goto('/login');
    await page.fill('input[type="email"]', 'test@playwright.com');
    await page.fill('input[type="password"]', 'TestPass123!');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL('/');
    await expect(page.locator('nav')).toBeVisible();
  });
});
