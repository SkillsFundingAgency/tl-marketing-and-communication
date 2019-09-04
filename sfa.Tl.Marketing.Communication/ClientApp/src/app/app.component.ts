import { Component, OnInit } from '@angular/core';

import { DataService } from '@core/services/user-data.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'new-tlevels';
  public pageTitle: string;

  constructor(private data: DataService) { }

  ngOnInit() {

    this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);

  }

  getOutputToParentValue(data: any) {
    this.pageTitle = data;
  }

}
