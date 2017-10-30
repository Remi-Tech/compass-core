import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';

// Import services
import { RevokeApplicationTokensService } from './providers/revoke-application-tokens.service';
import { ApplicationsService } from './providers/applications.service';

// Import model
import { Applications } from './models/applications.model';

@Component({
    selector: 'compass-at-applications',
    templateUrl: './applications.component.html',
    styleUrls: ['./applications.component.css']
})

export class ApplicationsComponent implements OnInit {

    public applicationsList: Applications[];

    public constructor(
        private applicationsService: ApplicationsService,
        private revokeApplicationTokensService: RevokeApplicationTokensService
    ) {

    }

    public ngOnInit(): void {
        this.applicationsService
            .view()
            .subscribe((data: Applications[]) => {
                this.applicationsList = data;
            },
            error => error);
    }

    public revokeAppToken(selectedApp: Applications): void {

        this.revokeApplicationTokensService.revokeAppToken(selectedApp);

    }
}

