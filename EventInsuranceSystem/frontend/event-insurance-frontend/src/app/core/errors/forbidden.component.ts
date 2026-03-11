import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-forbidden',
    imports: [CommonModule, RouterModule],
    templateUrl: './forbidden.component.html',
    styleUrl: './forbidden.component.scss'
})
export class ForbiddenComponent { }
