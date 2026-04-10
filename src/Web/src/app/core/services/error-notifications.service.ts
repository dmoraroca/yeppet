import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';

export interface ErrorNotification {
  id: number;
  title: string;
  message: string;
  createdAt: string;
  readAt: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorNotificationsService {
  private nextId = 1;
  private readonly notificationState = signal<ErrorNotification[]>([]);

  readonly notifications = this.notificationState.asReadonly();
  readonly unreadCount = computed(() =>
    this.notificationState().filter((notification) => notification.readAt === null).length
  );
  readonly hasUnread = computed(() => this.unreadCount() > 0);

  pushHttpError(error: HttpErrorResponse): void {
    const notification = this.buildHttpNotification(error);
    this.push(notification.title, notification.message);
  }

  pushUnexpectedError(message: string): void {
    this.push('Error inesperat', message);
  }

  notify(title: string, message: string): void {
    this.push(title, message);
  }

  dismiss(id: number): void {
    this.notificationState.update((items) => items.filter((item) => item.id !== id));
  }

  markAsRead(id: number): void {
    this.notificationState.update((items) =>
      items.map((item) =>
        item.id === id && item.readAt === null
          ? {
              ...item,
              readAt: new Date().toISOString()
            }
          : item
      )
    );
  }

  markAsUnread(id: number): void {
    this.notificationState.update((items) =>
      items.map((item) =>
        item.id === id
          ? {
              ...item,
              readAt: null
            }
          : item
      )
    );
  }

  markAllAsRead(): void {
    const now = new Date().toISOString();

    this.notificationState.update((items) =>
      items.map((item) =>
        item.readAt === null
          ? {
              ...item,
              readAt: now
            }
          : item
      )
    );
  }

  clear(): void {
    this.notificationState.set([]);
  }

  private push(title: string, message: string): void {
    const id = this.nextId++;

    this.notificationState.update((items) => [
      {
        id,
        title,
        message,
        createdAt: new Date().toISOString(),
        readAt: null
      },
      ...items
    ]);
  }

  private buildHttpNotification(error: HttpErrorResponse): Omit<ErrorNotification, 'id' | 'createdAt' | 'readAt'> {
    if (error.status === 0) {
      return {
        title: 'Sense connexio',
        message: 'No s’ha pogut contactar amb el servidor. Revisa la connexio i torna-ho a provar.'
      };
    }

    if (error.status === 401) {
      return {
        title: 'Sessio no autoritzada',
        message: 'Cal tornar a iniciar sessio per continuar.'
      };
    }

    if (error.status === 403) {
      return {
        title: 'Acces denegat',
        message: 'No tens permisos per accedir a aquest recurs.'
      };
    }

    if (error.status === 404) {
      return {
        title: 'Recurs no trobat',
        message: 'El recurs sollicitat no existeix o ja no esta disponible.'
      };
    }

    if (error.status >= 500) {
      return {
        title: 'Error del servidor',
        message: 'Hi ha hagut un problema intern. Torna-ho a provar mes endavant.'
      };
    }

    return {
      title: 'Error de peticio',
      message: error.message || 'La peticio no s’ha pogut completar correctament.'
    };
  }
}
