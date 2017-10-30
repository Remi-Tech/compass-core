import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Import services
import { RegisterService } from '../pages/register/register.service';
import { ApplicationsService } from '../pages/applications/providers/applications.service';
import { RevokeApplicationTokensService } from '../pages/applications/providers/revoke-application-tokens.service';

// Import modules from PrimeNG
import {
  MenuModule,
  InputTextModule,
  ButtonModule,
  DataTableModule,
  SharedModule
} from 'primeng/primeng';

// Import components
import { AppComponent } from './app.component';
import { RegisterComponent } from '../pages/register/register.component';
import { ApplicationsComponent } from '../pages/applications/applications.component';

const appRoutes: Routes = [

  { path: '', redirectTo: '/register', pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'view-applications', component: ApplicationsComponent }

];

@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    ApplicationsComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    MenuModule,
    InputTextModule,
    ButtonModule,
    DataTableModule,
    SharedModule,
    HttpModule,
    RouterModule.forRoot(appRoutes),

  ],
  providers: [RegisterService,
    ApplicationsService,
    RevokeApplicationTokensService],
  bootstrap: [AppComponent]
})

export class AppModule { }
