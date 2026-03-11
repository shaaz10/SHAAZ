import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-unauthorized',
    imports: [CommonModule, RouterModule],
    templateUrl: './unauthorized.component.html',
    styleUrl: './unauthorized.component.scss'
})
export class UnauthorizedComponent { }
