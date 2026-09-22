import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup } from './auth.helper.js';

test.describe('Gestión de Doctores', () => {
  test.beforeAll(async ({ request }) => {
    await registerTestUser(request);
  });

  test.beforeEach(async ({ page }) => {
    await loginAndSetup(page);
    await page.goto('/doctors');
  });

  test('debe mostrar la tabla de doctores', async ({ page }) => {
    await expect(page.locator('h2')).toHaveText('Doctores');
    await expect(page.locator('table thead th')).toHaveCount(5);
  });

  test('debe tener enlace para nuevo doctor', async ({ page }) => {
    const nuevoBtn = page.locator('a.btn').first();
    await expect(nuevoBtn).toHaveText(/Nuevo Doctor/);
    await expect(nuevoBtn).toHaveAttribute('href', '/doctors/new');
  });

  test('debe navegar al formulario de nuevo doctor', async ({ page }) => {
    await page.click('a.btn:has-text("Nuevo Doctor")');
    await expect(page).toHaveURL('/doctors/new');
    await expect(page.locator('h2')).toHaveText('Nuevo Doctor');
  });

  test('el formulario debe tener todos los campos', async ({ page }) => {
    await page.goto('/doctors/new');
    const inputs = page.locator('form input');
    await expect(inputs).toHaveCount(3);
    await expect(page.locator('form select')).toHaveCount(1);
    await expect(page.locator('button[type="submit"]')).toHaveText('Guardar doctor');
  });

  test('el desplegable de especialidad debe contener las 24 especialidades', async ({ page }) => {
    await page.goto('/doctors/new');
    const options = page.locator('form select option');
    await expect(options).toHaveCount(25);
    await expect(page.locator('select option[value="cardiologia"]')).toHaveText('Cardiología');
    await expect(page.locator('select option[value="pediatria"]')).toHaveText('Pediatría');
    await expect(page.locator('select option[value="medicina-interna"]')).toHaveText('Medicina Interna');
  });

  test('debe navegar desde el nav a doctores', async ({ page }) => {
    await page.goto('/');
    const nav = page.locator('nav a');
    const doctorLink = nav.filter({ hasText: 'Doctores' });
    await doctorLink.click();
    await expect(page).toHaveURL('/doctors');
  });

  test('debe crear un nuevo doctor correctamente', async ({ page }) => {
    const unique = Date.now();
    const newDoctor = {
      name: `Doctor Test ${unique}`,
      lastName: `Apellido Test ${unique}`,
      phone: `+34 600 ${unique}`.substring(0, 16),
      specialty: 'cardiologia',
    };

    await page.goto('/doctors/new');

    await page.fill('input[name="name"]', newDoctor.name);
    await page.fill('input[name="lastName"]', newDoctor.lastName);
    await page.fill('input[name="phone"]', newDoctor.phone);
    await page.selectOption('select[name="specialty"]', newDoctor.specialty);

    const submit = page.locator('button[type="submit"]');
    await expect(submit).toBeEnabled();
    await submit.click();

    await expect(page).toHaveURL('/doctors');

    const createdRow = page.locator('tbody tr', { hasText: newDoctor.name });
    await expect(createdRow).toBeVisible();
    await expect(createdRow).toContainText(newDoctor.lastName);
    await expect(createdRow).toContainText('Cardiología');

    await createdRow.locator('button.btn-danger').click();
    await expect(createdRow).toHaveCount(0);
  });

  test('debe editar un doctor existente', async ({ page }) => {
    const unique = Date.now();
    const token = await page.evaluate(() => localStorage.getItem('auth_token'));
    const createRes = await page.request.post('http://localhost:5091/api/doctors', {
      headers: { Authorization: `Bearer ${token}` },
      data: {
        name: `Doctor Edit ${unique}`,
        lastName: `Apellido Edit ${unique}`,
        phone: '+34 600 999 888',
        specialty: 'neurologia',
      },
    });
    expect(createRes.ok()).toBeTruthy();
    const created = await createRes.json();

    await page.goto(`/doctors/${created.id}/edit`);
    await expect(page.locator('h2')).toHaveText('Editar Doctor');

    const nameInput = page.locator('input[name="name"]');
    await expect(nameInput).toHaveValue(`Doctor Edit ${unique}`);

    const updatedName = `Doctor Actualizado ${unique}`;
    await nameInput.clear();
    await nameInput.fill(updatedName);
    await page.selectOption('select[name="specialty"]', 'dermatologia');

    await page.click('button[type="submit"]');

    await expect(page).toHaveURL('/doctors');

    const updatedRow = page.locator('tbody tr', { hasText: updatedName });
    await expect(updatedRow).toBeVisible();
    await expect(updatedRow).toContainText('Dermatología');

    await page.request.delete(`http://localhost:5091/api/doctors/${created.id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  });

  test('debe eliminar un doctor del listado', async ({ page }) => {
    const unique = Date.now();
    const token = await page.evaluate(() => localStorage.getItem('auth_token'));
    const createRes = await page.request.post('http://localhost:5091/api/doctors', {
      headers: { Authorization: `Bearer ${token}` },
      data: {
        name: `Doctor Delete ${unique}`,
        lastName: `Apellido Delete ${unique}`,
        phone: '+34 600 777 666',
        specialty: 'oftalmologia',
      },
    });
    expect(createRes.ok()).toBeTruthy();
    const created = await createRes.json();

    await page.reload();

    const row = page.locator('tbody tr', { hasText: `Doctor Delete ${unique}` });
    await expect(row).toBeVisible();
    await row.locator('button.btn-danger').click();
    await expect(row).toHaveCount(0);
  });

  test('debe deshabilitar el botón guardar con campos vacíos', async ({ page }) => {
    await page.goto('/doctors/new');
    const submit = page.locator('button[type="submit"]');
    await expect(submit).toBeDisabled();

    await page.fill('input[name="name"]', 'María');
    await expect(submit).toBeDisabled();

    await page.fill('input[name="lastName"]', 'García');
    await expect(submit).toBeDisabled();

    await page.fill('input[name="phone"]', '+34 600 123 456');
    await expect(submit).toBeDisabled();

    await page.selectOption('select[name="specialty"]', 'cardiologia');
    await expect(submit).toBeEnabled();
  });
});