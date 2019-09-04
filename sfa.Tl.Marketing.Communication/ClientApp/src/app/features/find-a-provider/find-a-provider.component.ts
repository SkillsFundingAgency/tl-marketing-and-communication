import { Component, OnInit } from '@angular/core';
import { IProvider } from '@core/models/provider.model';
import { ProvidersService } from '@core/services/providers.service';

import { DataService } from '@core/services/user-data.service';

@Component({
  selector: 'app-find-a-provider',
  templateUrl: './find-a-provider.component.html',
  styleUrls: ['./find-a-provider.component.scss']
})
export class FindAProviderComponent implements OnInit {

  public pageTitle = 'map';

  title = 'ESFAMarketingFrontEnd';
    curLatitude: number;
    curLongitude: number;
    curSubject: string;
    mapLatitude: number;
    mapLongitude: number;
    providers: IProvider[];
    curDistance: 0;
    map: any;
    distanceArray: number[] = [2, 5, 10, 15, 20, 30, 40, 50, 75, 100, 150, 200];

    subjectDisplayNames = {
        all: 'All Subjects',
        digital: 'Digital',
        construction: 'Construction',
        educationChildcare: 'Education and Childcare'
    };

    constructor(private providersService: ProvidersService, private data: DataService) { }

    refreshProviders(subject: string = 'all') {
      this.curSubject = subject;
      this.providers = this.providersService.getProviders(subject, this.curDistance, { lat: this.curLatitude, lng: this.curLongitude });
    }

    ngOnInit() {
      this.setPageTitle();
      this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);

      this.setCurrentLocation();
      this.refreshProviders();
    }

    setPageTitle() {
      this.data.updateState(this.pageTitle);
    }

    setCurrentLocation() {
        if ('geolocation' in navigator) {
          navigator.geolocation.getCurrentPosition((position) => {

              this.curLatitude = position.coords.latitude;
              this.curLongitude = position.coords.longitude;
          });
        }
    }

    markerClick(latitude: number, longitude: number) {
      this.mapLatitude = latitude;
      this.mapLongitude = longitude;

      if (this.map) {
        this.map.zoom = 9;
      }
    }

    distanceChange(newDistance) {
      this.curDistance = newDistance;
      this.refreshProviders(this.curSubject);
    }

    searchPostcode(postcode: string) {
        this.providersService.getGeocodeJson(postcode).subscribe(geocodes => {
          this.curLatitude = geocodes.results[0].geometry.location.lat;
          this.curLongitude = geocodes.results[0].geometry.location.lng;

          this.refreshProviders(this.curSubject);
        });
    }

    onMapReady(map) {
      this.map = map;
    }

    subjectChange(subject) {
      this.refreshProviders(subject);
    }

}
