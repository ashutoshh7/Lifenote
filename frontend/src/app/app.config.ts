import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection, SecurityContext, isDevMode } from '@angular/core';
import { provideRouter, withViewTransitions, withPreloading, PreloadAllModules } from '@angular/router';
import { provideServiceWorker } from '@angular/service-worker';
import { LucideAngularModule, CheckSquare, Timer, Repeat2, Settings, PanelLeftClose, PanelRightClose, StickyNote, StickyNoteIcon, Notebook, Trophy, Flame, Target, CheckCircle, Circle, Edit3, BarChart2, Sparkles, Plus, Sun, Moon, Monitor, Bell, User, Shield, Info, Trash2, X, Check, Pencil, Play, RotateCcw } from 'lucide-angular';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { provideMarkdown } from 'ngx-markdown';
import ClipboardJS from 'clipboard';
import 'prismjs';
import 'prismjs/plugins/line-numbers/prism-line-numbers';
import 'prismjs/plugins/command-line/prism-command-line';
import 'prismjs/components/prism-typescript.min.js';
import 'prismjs/components/prism-javascript.min.js';
import 'prismjs/components/prism-json.min.js';
import 'prismjs/components/prism-css.min.js';
import 'prismjs/components/prism-markup.min.js';
import 'prismjs/components/prism-csharp.min.js';
import 'prismjs/components/prism-bash.min.js';
import 'prismjs/components/prism-python.min.js';


export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withViewTransitions(), withPreloading(PreloadAllModules)),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerImmediately'
    }),
    importProvidersFrom(LucideAngularModule.pick({ Timer, Repeat2, Settings, PanelLeftClose, PanelRightClose, StickyNote, StickyNoteIcon, Notebook, Trophy, Flame, Target, CheckCircle, Circle, Edit3, BarChart2, Sparkles, Plus, Sun, Moon, Monitor, Bell, User, Shield, Info, Trash2, X, Check, Pencil, Play, RotateCcw })),
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor]) // <-- Add your generated interceptor here
    ),
    provideMarkdown()
  ]
};
