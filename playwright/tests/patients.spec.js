import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup } from './auth.helper.js';

test.describe('Navegación - Pacientes', () => {
  test.beforeAll(async ({ request }) => {
    await registerTestUser(request);
  });

  test.beforeEach(async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/patients');
  });

  test('debe mostrar la lista de pacientes', async ({ page }) => {
    await expect(page.locator('h2')).toHaveText('Pacientes');
    await expect(page.locator('table thead th')).toHaveCount(4);
  });

  test('debe tener enlace para nuevo paciente', async ({ page }) => {
    const nuevoBtn = page.locator('a.btn');
    await expect(nuevoBtn).toHaveText('Nuevo Paciente');
    await expect(nuevoBtn).toHaveAttribute('href', '/patients/new');
  });

  test('debe navegar al formulario de nuevo paciente', async ({ page }) => {
    await page.click('a.btn');
    await expect(page).toHaveURL('/patients/new');
    await expect(page.locator('h2')).toHaveText('Nuevo Paciente');
  });

  test('el formulario de paciente debe tener todos los campos', async ({ page }) => {
    await page.goto('/patients/new');
    const inputs = page.locator('form input');
    await expect(inputs).toHaveCount(5);
    await expect(page.locator('button[type="submit"]')).toHaveText('Guardar paciente');
  });

  test('debe navegar desde el nav a pacientes', async ({ page }) => {
    await page.goto('/');
    await page.locator('nav a').nth(1).click();
    await expect(page).toHaveURL('/patients');
  });

  test('debe crear un nuevo paciente correctamente', async ({ page }) => {
    const unique = Date.now();
    const newPatient = {
      name: `Paciente Test ${unique}`,
      email: `paciente${unique}@test.com`,
      phone: '+52 55 1234 5678',
      dateOfBirth: '1990-05-15',
      address: 'Calle Prueba 123, Ciudad',
    };

    await page.goto('/patients/new');

    await page.fill('input[name="name"]', newPatient.name);
    await page.fill('input[name="email"]', newPatient.email);
    await page.fill('input[name="phone"]', newPatient.phone);
    await page.fill('input[name="dateOfBirth"]', newPatient.dateOfBirth);
    await page.fill('input[name="address"]', newPatient.address);

    const submit = page.locator('button[type="submit"]');
    await expect(submit).toBeEnabled();
    await submit.click();

    await expect(page).toHaveURL('/patients');

    const createdRow = page.locator('tbody tr', { hasText: newPatient.name });
    await expect(createdRow).toBeVisible();
    await expect(createdRow).toContainText(newPatient.email);
    await expect(createdRow).toContainText(newPatient.phone);

    await createdRow.locator('button.btn-danger').click();
    await expect(createdRow).toHaveCount(0);
  });
});
