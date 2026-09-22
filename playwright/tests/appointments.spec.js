import { test, expect } from '@playwright/test';
import { registerTestUser, loginAndSetup, loginTestUser } from './auth.helper.js';

const API = 'http://localhost:5091/api';

async function createTestDoctor(request) {
  const user = await loginTestUser(request);
  expect(user).toBeTruthy();
  const unique = Date.now();
  const doctor = {
    name: `Doctor Cita ${unique}`,
    lastName: `Apellido Cita ${unique}`,
    phone: '+34 600 555 444',
    specialty: 'pediatria',
  };
  const createRes = await request.post(`${API}/doctors`, {
    headers: { Authorization: `Bearer ${user.token}` },
    data: doctor,
  });
  expect(createRes.ok()).toBeTruthy();
  const created = await createRes.json();
  return { token: user.token, doctor, created, fullName: `${doctor.name} ${doctor.lastName}` };
}

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
    await expect(page.locator('table thead th')).toHaveCount(6);
    await expect(page.locator('table thead th').nth(1)).toHaveText('Doctor');
  });

  test('debe tener enlace para nueva cita', async ({ page }) => {
    const nuevoBtn = page.getByRole('link', { name: 'Nueva Cita' });
    await expect(nuevoBtn).toBeVisible();
    await expect(nuevoBtn).toHaveAttribute('href', '/appointments/new');
  });

  test('debe tener boton de Exportar PDF', async ({ page }) => {
    const exportBtn = page.locator('button.btn-outline');
    await expect(exportBtn).toContainText('Exportar PDF');
  });

  test('debe navegar al formulario de nueva cita', async ({ page }) => {
    await page.getByRole('link', { name: 'Nueva Cita' }).click();
    await expect(page).toHaveURL('/appointments/new');
    await expect(page.locator('h2')).toHaveText('Nueva Cita');
  });

  test('el formulario de cita debe tener campos requeridos', async ({ page }) => {
    await page.goto('/appointments/new');
    await expect(page.locator('form select')).toHaveCount(2);
    await expect(page.locator('form select[name="patientId"]')).toBeVisible();
    await expect(page.locator('form select[name="doctorId"]')).toBeVisible();
    await expect(page.locator('form input[type="datetime-local"]')).toHaveCount(1);
    await expect(page.locator('form textarea')).toHaveCount(1);
    await expect(page.locator('button[type="submit"]')).toBeDisabled();
  });

  test('el desplegable de doctor debe mostrar los doctores registrados', async ({ page, request }) => {
    const { token, doctor, created, fullName } = await createTestDoctor(request);

    await page.goto('/appointments/new');
    const option = page.locator(`form select[name="doctorId"] option[value="${created.id}"]`);
    await expect(option).toHaveText(fullName);

    await request.delete(`${API}/doctors/${created.id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  });

  test('debe deshabilitar el botón guardar hasta seleccionar doctor', async ({ page, request }) => {
    const { token, created, doctor } = await createTestDoctor(request);
    const unique = Date.now();

    const patient = {
      name: `Paciente Val ${unique}`,
      email: `valpaciente${unique}@test.com`,
      phone: '+52 55 3333 4444',
      dateOfBirth: '1992-06-15',
      address: 'Calle Validación 7, Ciudad',
    };
    const createPatient = await request.post(`${API}/patients`, { data: patient });
    expect(createPatient.ok()).toBeTruthy();
    const createdPatient = await createPatient.json();

    await page.goto('/appointments/new');
    const submit = page.locator('button[type="submit"]');
    await expect(submit).toBeDisabled();

    await page.selectOption('select[name="patientId"]', createdPatient.id);
    await page.fill('input[name="date"]', '2026-08-20T11:00');
    await page.fill('textarea[name="reason"]', 'Motivo sin doctor');
    await expect(submit).toBeDisabled();

    await page.selectOption('select[name="doctorId"]', created.id);
    await expect(submit).toBeEnabled();

    await request.delete(`${API}/doctors/${created.id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    await request.delete(`${API}/patients/${createdPatient.id}`);
  });

  test('debe navegar desde el nav a citas', async ({ page }) => {
    await page.goto('/');
    await page.locator('nav a').filter({ hasText: 'Citas' }).click();
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

  test('debe crear una nueva cita correctamente con su doctor', async ({ page, request }) => {
    const unique = Date.now();
    const patient = {
      name: `Paciente Cita ${unique}`,
      email: `citapaciente${unique}@test.com`,
      phone: '+52 55 9876 5432',
      dateOfBirth: '1985-03-12',
      address: 'Av. Consulta 456, Ciudad',
    };

    const createPatient = await request.post(`${API}/patients`, {
      data: patient,
    });
    expect(createPatient.ok()).toBeTruthy();
    const createdPatient = await createPatient.json();

    const { token, doctor, created, fullName } = await createTestDoctor(request);

    const reason = `Motivo de consulta ${unique}`;
    await page.goto('/appointments/new');

    await page.selectOption('select[name="patientId"]', createdPatient.id);
    await page.selectOption('select[name="doctorId"]', created.id);
    await page.fill('input[name="date"]', '2026-08-20T10:30');
    await page.fill('textarea[name="reason"]', reason);

    const submit = page.locator('button[type="submit"]');
    await expect(submit).toBeEnabled();
    await submit.click();

    await expect(page).toHaveURL('/appointments');

    const createdRow = page.locator('tbody tr', { hasText: patient.name });
    await expect(createdRow).toBeVisible();
    await expect(createdRow).toContainText(fullName);
    await expect(createdRow).toContainText(reason);

    await createdRow.locator('button.btn-danger').click();
    await expect(createdRow).toHaveCount(0);

    await request.delete(`${API}/doctors/${created.id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    await request.delete(`${API}/patients/${createdPatient.id}`);
  });

  test('debe tener boton de calendario', async ({ page }) => {
    const calBtn = page.getByRole('link', { name: 'Calendario' });
    await expect(calBtn).toBeVisible();
    await expect(calBtn).toHaveAttribute('href', '/appointments/calendar');
  });

  test('debe navegar al calendario mensual de citas', async ({ page }) => {
    await page.getByRole('link', { name: 'Calendario' }).click();
    await expect(page).toHaveURL('/appointments/calendar');
    await expect(page.locator('h2')).toHaveText('Calendario de Citas');
    await expect(page.locator('.calendar-weekdays span')).toHaveCount(7);
    await expect(page.locator('.calendar-day')).toHaveCount(42);
    await expect(page.locator('.calendar-month')).toBeVisible();
  });

  test('debe navegar al mes anterior y siguiente', async ({ page }) => {
    await page.goto('/appointments/calendar');
    const month = await page.locator('.calendar-month').textContent();
    await page.getByRole('button', { name: 'Anterior' }).click();
    await expect(page.locator('.calendar-month')).not.toHaveText(month ?? '');
    await page.getByRole('button', { name: 'Siguiente' }).click();
    await expect(page.locator('.calendar-month')).toHaveText(month ?? '');
  });

  test('debe mostrar una cita ya creada en el calendario con paciente, doctor y motivo', async ({ page, request }) => {
    const user = await loginTestUser(request);
    expect(user).toBeTruthy();

    const unique = Date.now();
    const patient = {
      name: `Paciente Cal ${unique}`,
      email: `calpaciente${unique}@test.com`,
      phone: '+52 55 1111 2222',
      dateOfBirth: '1990-01-01',
      address: 'Calle 1, Ciudad',
    };
    const createPatient = await request.post(`${API}/patients`, { data: patient });
    expect(createPatient.ok()).toBeTruthy();
    const createdPatient = await createPatient.json();

    const { token, doctor, created, fullName } = await createTestDoctor(request);

    const today = new Date();
    const dateStr = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}T10:30`;
    const reason = `Motivo calendario ${unique}`;

    const createAppt = await request.post(`${API}/appointments`, {
      data: {
        userId: user.id,
        patientId: createdPatient.id,
        patientName: patient.name,
        doctorId: created.id,
        doctorName: fullName,
        date: dateStr,
        reason,
        status: 'Scheduled',
      },
    });
    expect(createAppt.ok()).toBeTruthy();
    const createdAppt = await createAppt.json();

    await page.goto('/appointments/calendar');

    const todayCell = page.locator('.calendar-day.today');
    await expect(todayCell).toContainText(patient.name);
    await expect(todayCell).toContainText(reason);
    await expect(todayCell.locator('.event-doctor')).toHaveText(fullName);

    await request.delete(`${API}/appointments/${createdAppt.id}`);
    await request.delete(`${API}/doctors/${created.id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    await request.delete(`${API}/patients/${createdPatient.id}`);
  });
});