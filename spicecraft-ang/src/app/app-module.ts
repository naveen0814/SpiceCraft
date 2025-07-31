import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { HomepageComponent } from './component/homepage/homepage';
import { App } from './app';

@NgModule({
  declarations: [App],
  imports: [
    BrowserModule,
    CommonModule,
    HttpClientModule,
    HomepageComponent,
    RouterModule.forRoot([
      { path: '', component: HomepageComponent },
      { path: '**', redirectTo: '' }
    ])
  ],
  bootstrap: [App]
})
export class AppModule {}
