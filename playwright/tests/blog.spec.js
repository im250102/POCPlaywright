import { test, expect } from '@playwright/test';

const { registerTestUser, loginAndSetup } = await import('./auth.helper.js');

test.describe.configure({ mode: 'serial' });

test.describe('Blog', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/blog');
  });

  test('cualquier visitante puede ver el listado sin estar autenticado', async ({ page }) => {
    await expect(page).toHaveURL('/blog');
    await expect(page.locator('h2')).toHaveText('Blog');
  });

  test('un visitante no puede crear ni editar posts', async ({ page }) => {
    await page.goto('/blog/new');
    await expect(page).toHaveURL('/login');

    await page.goto('/blog/inexistente/edit');
    await expect(page).toHaveURL('/login');
  });

  test('flujo completo: crear, leer, editar, comentar y eliminar un post', async ({ page, request }) => {
    const title = `Post E2E ${Date.now()}`;
    const editedTitle = `${title} (editado)`;

    await registerTestUser(request);
    await loginAndSetup(page);
    await page.goto('/blog');

    await page.locator('a.btn', { hasText: 'Nuevo Post' }).click();
    await expect(page).toHaveURL(/\/blog\/new/);

    await page.getByPlaceholder('Título del post').fill(title);
    await page.locator('select').selectOption('Tecnologia');
    await page.getByPlaceholder('Escribe el contenido de tu post...').fill('Contenido del post E2E con enlaces e imagenes');
    await page.getByRole('button', { name: 'Publicar' }).click();

    await expect(page).toHaveURL(/\/blog\/[^/]+$/);
    await expect(page.locator('h1')).toHaveText(title);
    await expect(page.locator('.post-content')).toContainText('Contenido del post E2E');

    await page.getByRole('link', { name: 'Editar' }).click();
    await expect(page).toHaveURL(/\/blog\/.+\/edit$/);
    await expect(page.getByPlaceholder('Título del post')).toHaveValue(title);

    await page.getByPlaceholder('Título del post').fill(editedTitle);
    await page.getByRole('button', { name: 'Guardar cambios' }).click();
    await expect(page.locator('h1')).toHaveText(editedTitle);
    await expect(page.locator('.post-meta')).toContainText('editado');

    await page.getByPlaceholder('Escribe un comentario...').fill('Excelente articulo');
    await page.getByRole('button', { name: 'Comentar' }).click();
    await expect(page.locator('.comment')).toHaveCount(1);
    await expect(page.locator('.comment')).toContainText('Excelente articulo');

    await page.getByRole('button', { name: 'Eliminar' }).click();
    await expect(page).toHaveURL(/\/blog$/);
    await expect(page.locator('.post-card', { hasText: editedTitle })).toHaveCount(0);
  });
});