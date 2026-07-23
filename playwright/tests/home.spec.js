import { test, expect } from '@playwright/test';

test.describe('Navegación - Home', () => {
  test('debe mostrar la página de inicio con las tarjetas de navegación', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toHaveText('Consultorio Médico');
    await expect(page.locator('.card')).toHaveCount(3);
    await expect(page.locator('.card').first()).toContainText('Pacientes');
  });

  test('debe navegar a Pacientes desde Home', async ({ page }) => {
    await page.goto('/');
    await page.click('a.card >> nth=0');
    await expect(page).toHaveURL(/\/patients/);
    await expect(page.locator('h2')).toHaveText('Pacientes');
  });

  test('debe navegar a Citas desde Home', async ({ page }) => {
    await page.goto('/');
    await page.click('a.card >> nth=1');
    await expect(page).toHaveURL(/\/appointments/);
    await expect(page.locator('h2')).toHaveText('Citas Médicas');
  });

  test('debe navegar a Informes desde Home', async ({ page }) => {
    await page.goto('/');
    await page.click('a.card >> nth=2');
    await expect(page).toHaveURL(/\/reports/);
    await expect(page.locator('h2')).toHaveText('Informes Médicos por Paciente');
  });

  test('la barra de navegación debe tener todos los enlaces', async ({ page }) => {
    await page.goto('/');
    const navLinks = page.locator('nav a');
    await expect(navLinks).toHaveCount(4);
    await expect(navLinks.nth(0)).toHaveText('Inicio');
    await expect(navLinks.nth(1)).toHaveText('Pacientes');
    await expect(navLinks.nth(2)).toHaveText('Citas');
    await expect(navLinks.nth(3)).toHaveText('Informes');
  });
});
