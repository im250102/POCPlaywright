import { test, expect } from '@playwright/test';

test.describe('Navegación - Informes', () => {
  test('debe mostrar lista de pacientes para informes', async ({ page }) => {
    await page.goto('/reports');
    await expect(page.locator('h2')).toHaveText('Informes Médicos por Paciente');
    await expect(page.locator('table thead th')).toHaveCount(3);
  });

  test('cada paciente debe tener enlace para ver informes', async ({ page }) => {
    await page.goto('/reports');
    const verBtns = page.locator('a.btn');
    const count = await verBtns.count();
    for (let i = 0; i < count; i++) {
      await expect(verBtns.nth(i)).toHaveText('Ver Informes');
    }
  });

  test('debe navegar desde el nav a informes', async ({ page }) => {
    await page.goto('/');
    await page.locator('nav a').nth(3).click();
    await expect(page).toHaveURL('/reports');
  });
});
