import { test, expect } from '@playwright/test';

test.describe('YepPet - Seguretat i Control d\'Accessos (Fase IV)', () => {

  test('JP-011/012: L\'Administrador ha de veure el menú de gestió interna', async ({ page }) => {
    await test.step('Login com a ADMIN', async () => {
      await page.goto('/login');
      await page.fill('input[formcontrolname="email"]', 'admin@admin.adm');
      await page.fill('input[formcontrolname="password"]', 'Admin123');
      await page.click('button[type="submit"]');
    });

    await test.step('Verificar presència del menú ADMIN', async () => {
      const adminMenu = page.locator('button:has-text("Administració"), .nav-link:has-text("ADMIN")');
      await expect(adminMenu).toBeVisible();
      await adminMenu.click();
      
      // Verifiquem opcions segons funcional-ca.md
      await expect(page.locator('a:has-text("Usuaris")')).toBeVisible();
      await expect(page.locator('a:has-text("Documentació")')).toBeVisible();
      // Screenshot: Menú d'administració desplegat
    });
  });

  test('JP-018: L\'usuari estàndard (USER) no té accés a la zona interna', async ({ page }) => {
    await test.step('Login com a USER', async () => {
      await page.goto('/login');
      await page.fill('input[formcontrolname="email"]', 'user@user.com');
      await page.fill('input[formcontrolname="password"]', 'Admin123');
      await page.click('button[type="submit"]');
    });

    await test.step('Verificar absència de menú ADMIN', async () => {
      await expect(page.locator('button:has-text("Administració")')).not.toBeVisible();
      // Screenshot: Header d'usuari estàndard sense opcions d'admin
    });

    await test.step('Intentar accés forçat per URL (JP-031)', async () => {
      await page.goto('/admin/usuaris');
      // El permissionGuard ha de redirigir fora
      await expect(page).not.toHaveURL(/\/admin\/usuaris/);
      await expect(page).toHaveURL(/\/places|\/home|\/perfil/);
    });
  });

  test('JP-015: El rol VIEWER té restringida l\'escriptura', async ({ page }) => {
    // Nota: Aquí assumim que tenim un usuari amb rol VIEWER per a la prova
    // Si no, l'hauríem de crear en un pas previ.
    await test.step('Navegar a un detall de lloc com a anònim/viewer', async () => {
      await page.goto('/places');
      await page.click('app-place-card:first-child a');
    });

    await test.step('Verificar que l\'acció de Favorit no és operativa o redirigeix', async () => {
      const favButton = page.locator('app-favorite-toggle-button').first();
      if (await favButton.isVisible()) {
        await favButton.click();
        // Segons el flux de Fase IV, hauria de sortir un error o demanar login/permís
        await expect(page.locator('app-error-notifications')).toBeVisible();
      }
      // Screenshot: Intent d'acció restringida per a VIEWER
    });
  });

});