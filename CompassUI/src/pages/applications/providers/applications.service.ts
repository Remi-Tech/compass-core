import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

// Import environment variable
import { environment } from '../../../environments/environment';

// Import model
import { Applications } from '../../applications/models/applications.model';

// Import rxjs
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class ApplicationsService {

    public viewUrl: string = 'config/applications';

    public constructor(
        private http: Http
    ) { }

    public view = (): Observable<Applications[]> => {
        return this.http.get(environment.CompassUrl + this.viewUrl)
            .map(data => data.json());
    }
}
