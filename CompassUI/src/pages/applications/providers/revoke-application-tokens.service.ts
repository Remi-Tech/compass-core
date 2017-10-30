import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

// Import environment variable
import { environment } from '../../../environments/environment';

// Import model
import { Applications } from '../../applications/models/applications.model';

// Import rxjs
import 'rxjs/add/operator/toPromise';

@Injectable()
export class RevokeApplicationTokensService {

    public revokeUrl: string = 'config/revoke/';

    public constructor(
        private http: Http
    ) { }

    public revokeAppToken(appToken: Applications): void {
        this.http.post(environment.CompassUrl + this.revokeUrl + appToken.applicationToken, null)
            .toPromise()
            .then(result => {
                result.json();
            })
            .catch(err => {
                err.json();
            });
    }
}
