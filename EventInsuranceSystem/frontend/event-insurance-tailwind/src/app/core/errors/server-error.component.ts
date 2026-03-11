import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-server-error',
    imports: [CommonModule, RouterModule],
    templateUrl: './server-error.component.html',
    styleUrl: './server-error.component.css'
})
export class ServerErrorComponent { }
