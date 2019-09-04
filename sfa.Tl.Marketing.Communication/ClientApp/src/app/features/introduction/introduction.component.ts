import { Component, OnInit } from '@angular/core';

import { DataService } from '@core/services/user-data.service';


@Component({
  selector: 'app-introduction',
  templateUrl: './introduction.component.html',
  styleUrls: ['./introduction.component.scss']
})
export class IntroductionComponent implements OnInit {

  public pageTitle = 'introduction';

  constructor(private data: DataService) { }

  ngOnInit() {
    this.setPageTitle();
    this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);
  }

  setPageTitle() {
    this.data.updateState(this.pageTitle);
  }

}
