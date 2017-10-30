import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

// Import environment variable
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class RegisterService {

    public registerUrl: string = 'config/register/';

    public constructor(
        private http: Http
    ) { }

    public create(name: string): void {
        this.http.post(environment.CompassUrl + this.registerUrl + name, null)
        .toPromise()
        .then(result => {
            result.json();
        })
        .catch(err => {
            err.json();
        });
    }

}
