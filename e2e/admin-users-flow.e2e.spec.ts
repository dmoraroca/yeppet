import { test, expect } from '@playwright/test';

test.describe('YepPet - Administració - Manteniment d\'usuaris', () => {
  
  test.beforeEach(async ({ page }) => {
    await test.step('Accés amb credencials d\'Administrador', async () => {
      await page.goto('/login');
      await page.fill('input[formcontrolname="email"]', 'admin@admin.adm');
      await page.fill('input[formcontrolname="password"]', 'Admin123');
      await page.click('button[type="submit"]');
      await expect(page).not.toHaveURL('/login');
    });
  });

  test('ha de permetre el flux complet: crear, editar i esborrar un usuari', async ({ page }) => {
    const tempEmail = `test.e2e.${Date.now()}@yeppet.com`;
    const tempName = 'Usuari de Prova E2E';

    await test.step('Navegar al manteniment d\'usuaris', async () => {
      await page.goto('/admin/usuaris');
      await expect(page.locator('h2')).toContainText('Gestió d\'usuaris');
      // Screenshot: Pantalla principal de la graella d'usuaris
    });

    await test.step('Obrir formulari d\'alta i informar dades', async () => {
      await page.click('button:has-text("Crear usuari")');
      
      await page.fill('#admin-create-user-form input[formcontrolname="email"]', tempEmail);
      await page.fill('#admin-create-user-form input[formcontrolname="password"]', 'Pass123!');
      await page.fill('#admin-create-user-form input[formcontrolname="displayName"]', tempName);
      await page.fill('#admin-create-user-form input[formcontrolname="city"]', 'Barcelona');
      await page.fill('#admin-create-user-form input[formcontrolname="country"]', 'Espanya');
      
      await page.check('input[type="checkbox"]'); // Acceptació privacitat interna
      // Screenshot: Formulari d'alta emplenat
      
      await page.click('button[type="submit"]:has-text("Crear")');
    });

    await test.step('Verificar creació i obrir detall', async () => {
      await expect(page.locator('.admin-console-success-popup')).toBeVisible();
      await page.click('.admin-console-success-popup'); // Tancar popup d'èxit

      const userRow = page.locator(`tr:has-text("${tempEmail}")`);
      await expect(userRow).toBeVisible();
      
      await userRow.click();
      await expect(page.locator('h3:has-text("Detall d\'usuari")')).toBeVisible();
      // Screenshot: Vista de detall de l'usuari acabat de crear
    });

    await test.step('Modificar dades de l\'usuari', async () => {
      await page.click('button:has-text("Modificar")');
      await page.fill('textarea[formcontrolname="bio"]', 'Bio editada rigurosament per a documentació.');
      
      // Segons codi cal acceptar privacitat per a cada canvi persistent
      await page.check('.admin-console-modal--detail input[type="checkbox"]');
      
      await page.click('button:has-text("Desar")');
      await expect(page.locator('app-error-notifications')).toContainText('Usuari actualitzat');
      // Screenshot: Notificació d'usuari actualitzat
    });

    await test.step('Executar baixa de l\'usuari', async () => {
      // Tanquem detall si cal
      await page.keyboard.press('Escape');
      
      const userRow = page.locator(`tr:has-text("${tempEmail}")`);
      await userRow.locator('button[title="Esborrar usuari"]').click();
      
      await expect(page.locator('.admin-console-modal--confirm')).toBeVisible();
      // Screenshot: Modal de confirmació d'esborrat
      
      await page.click('button:has-text("Esborrar")');
      await expect(page.locator(`tr:has-text("${tempEmail}")`)).not.toBeVisible();
    });
  });
});
