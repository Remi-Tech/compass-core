import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

// Import from PrimeNG
import { MenuItem } from 'primeng/primeng';
import { Menu } from 'primeng/components/menu/menu';


@Component({
  selector: 'compass-app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {

  public menuItems: MenuItem[];

  public constructor(
    private router: Router
  ) { }

  public ngOnInit(): void {

    this.menuItems = [
      { label: 'Register', icon: 'fa-plus', routerLink: ['/register'] },
      { label: 'View All Applications', icon: 'fa-files-o', routerLink: ['/view-applications'] }
    ];
  }

}
