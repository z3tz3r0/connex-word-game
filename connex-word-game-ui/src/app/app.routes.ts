import { Routes } from '@angular/router';
import { Game } from './pages/game/game';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { authGuard } from './services/auth-guard';

export const routes: Routes = [
  { path: '', component: Game, canActivate: [authGuard] },
  { path: 'game', component: Game, canActivate: [authGuard] },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
];
