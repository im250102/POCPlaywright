import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup } from './auth.helper.js';

test.describe('Navegación - Informes', () => {
  test.beforeAll(async ({ request }) => {
    await registerTestUser(request);
  });

  test.beforeEach(async ({ page }) => {
    await loginAndSetup(page);
  });

  test('debe mostrar lista de pacientes para informes', async ({ page }) => {
    await page.goto('/reports');
    await expect(page.locator('h2')).toHaveText('Informes Médicos');
    await expect(page.locator('table thead th')).toHaveCount(3);
  });

  test('cada paciente debe tener enlace para ver informes', async ({ page }) => {
    await page.goto('/reports');
    const verBtns = page.locator('a.btn');
    const count = await verBtns.count();
    for (let i = 0; i < count; i++) {
      await expect(verBtns.nth(i)).toHaveText('Ver informes');
    }
  });

  test('debe navegar a vista de informes del paciente', async ({ page }) => {
    await page.goto('/reports');
    const firstPatientLink = page.locator('a.btn').first();
    await firstPatientLink.click();
    await expect(page).toHaveURL(/\/reports\/.+/);
    await expect(page.locator('h2')).toHaveText('Informes del Paciente');
  });

  test('debe tener boton Exportar PDF en cada informe', async ({ page }) => {
    await page.goto('/reports');
    await page.locator('a.btn').first().click();
    const exportBtns = page.locator('button.btn-outline').filter({ hasText: 'Exportar PDF' });
    const count = await exportBtns.count();
    for (let i = 0; i < count; i++) {
      await expect(exportBtns.nth(i)).toBeVisible();
    }
  });

  test('debe navegar desde el nav a informes', async ({ page }) => {
    await page.goto('/');
    await page.locator('nav a').nth(3).click();
    await expect(page).toHaveURL('/reports');
  });
});
