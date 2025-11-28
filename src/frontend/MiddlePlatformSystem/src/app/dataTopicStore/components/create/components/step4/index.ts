import { Component, Input, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-data-topic-edit-step4',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService],
})
export class DataTopicEditStep4Component implements OnInit {
  @Input() field: any;
  isView: boolean = false
  constructor(
    private readonly messageService: MessageService,
  ) { }

  public ngOnInit() { }
}
