import { Component, OnInit, ElementRef } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

// Import service
import { RegisterService } from './register.service';

@Component({
    selector: 'compass-at-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
    
    public appForm: FormGroup;

    public constructor(
        private fb: FormBuilder, 
        private registerService: RegisterService
    ) { }

    public addAppName(appName: string): void {
        appName = appName;
        this.registerService.create(appName);
    }

    public ngOnInit(): void {
        this.appForm = this.fb.group({
            appName: ['', [Validators.required]]
        });
    }

    // TODO: Get Guid - appToken to display
    // TODO: Error handling when adding and the name already exists
    
}
