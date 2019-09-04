import { Component, OnInit } from '@angular/core';

import { DataService } from '@core/services/user-data.service';

@Component({
  selector: 'app-subjects',
  templateUrl: './subjects.component.html',
  styleUrls: ['./subjects.component.scss']
})
export class SubjectsComponent implements OnInit {

  public pageTitle = 'subjects';

  constructor(private data: DataService) { }

  ngOnInit() {
    this.setPageTitle();
    this.data.routingData$.subscribe(pageTitle => this.pageTitle = pageTitle);
  }

  setPageTitle() {
    this.data.updateState(this.pageTitle);
  }

}
