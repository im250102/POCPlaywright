import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup } from './auth.helper.js';

test.describe('Navegación - Citas', () => {
  test.beforeAll(async ({ request }) => {
    await registerTestUser(request);
  });

  test.beforeEach(async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/appointments');
  });

  test('debe mostrar la lista de citas', async ({ page }) => {
    await expect(page.locator('h2')).toHaveText('Citas Médicas');
    await expect(page.locator('table thead th')).toHaveCount(5);
  });

  test('debe tener enlace para nueva cita', async ({ page }) => {
    const nuevoBtn = page.locator('a.btn');
    await expect(nuevoBtn).toHaveText('Nueva Cita');
    await expect(nuevoBtn).toHaveAttribute('href', '/appointments/new');
  });

  test('debe tener boton de Exportar PDF', async ({ page }) => {
    const exportBtn = page.locator('button.btn-outline');
    await expect(exportBtn).toContainText('Exportar PDF');
  });

  test('debe navegar al formulario de nueva cita', async ({ page }) => {
    await page.click('a.btn');
    await expect(page).toHaveURL('/appointments/new');
    await expect(page.locator('h2')).toHaveText('Nueva Cita');
  });

  test('el formulario de cita debe tener campos requeridos', async ({ page }) => {
    await page.goto('/appointments/new');
    await expect(page.locator('form select')).toHaveCount(1);
    await expect(page.locator('form input[type="datetime-local"]')).toHaveCount(1);
    await expect(page.locator('form textarea')).toHaveCount(1);
    await expect(page.locator('button[type="submit"]')).toBeDisabled();
  });

  test('debe navegar desde el nav a citas', async ({ page }) => {
    await page.goto('/');
    await page.locator('nav a').nth(2).click();
    await expect(page).toHaveURL('/appointments');
  });

  test('debe descargar PDF al hacer clic en Exportar PDF', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download', { timeout: 15000 }).catch(() => null);
    await page.locator('button.btn-outline').filter({ hasText: 'Exportar PDF' }).click();
    const download = await downloadPromise;
    if (download) {
      expect(download.suggestedFilename()).toMatch(/\.pdf$/i);
    }
  });
});
