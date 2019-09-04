import { Component, OnInit, Output, EventEmitter } from '@angular/core';

import { DataService } from '@core/services/user-data.service';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.scss']
})
export class HomepageComponent implements OnInit {
  public pageTitle = 'home';

  constructor(private data: DataService) { }

  ngOnInit() {
    this.setPageTitle();
    this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);
  }

  setPageTitle() {
    this.data.updateState(this.pageTitle);
  }

}
