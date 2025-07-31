import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.html',
  styleUrls: ['./homepage.css']
})
export class HomepageComponent implements OnInit {
  restaurants: any[] = [];
  role: string | null = null;
  readonly PRIMARY = '#F05423';
  currentYear = new Date().getFullYear();

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  async load(): Promise<void> {
    try {
      const data: any = await this.http.get('/api/Restaurant').toPromise();
      this.restaurants = data;
    } catch (err) {
      console.error(err);
      window.alert('Failed to load restaurants');
    }

    const token = localStorage.getItem('token');
    if (token) {
      try {
        const dec: any = jwtDecode(token);
        this.role = dec.role || dec['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'];
      } catch {
        this.role = null;
      }
    }
  }

  handleOrder(rid: number): void {
    if (!this.role) {
      window.alert('Please log in to place an order');
      this.router.navigate(['/login']);
    } else {
      this.router.navigate([`/restaurant/${rid}/menu`]);
    }
  }

  goTo(path: string): void {
    this.router.navigate([path]);
  }

  onLogoError(event: Event): void {
    (event.target as HTMLImageElement).src = '/images/logo.png';
  }
}
