import { Component, OnInit } from '@angular/core';

import { DataService } from '@core/services/user-data.service';

@Component({
  selector: 'app-benefits',
  templateUrl: './benefits.component.html',
  styleUrls: ['./benefits.component.scss']
})
export class BenefitsComponent implements OnInit {

  public pageTitle = 'benefits';

  constructor(private data: DataService) { }

  ngOnInit() {
    this.setPageTitle();
    this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);
  }

  setPageTitle() {
    this.data.updateState(this.pageTitle);
  }

}
