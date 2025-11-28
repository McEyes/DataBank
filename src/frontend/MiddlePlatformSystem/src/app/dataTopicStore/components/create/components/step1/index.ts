import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import LoginHttpService from 'src/api/common/login';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-topic-edit-step1',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, LoginHttpService],
})
export class DataTopicEditStep1Component implements OnInit {

  editorValue: string = ''
  readonly: boolean = false
  constructor(
    public httpService: LoginHttpService,
    private readonly router: Router,
    private readonly messageService: MessageService,
  ) { }

  public async ngOnInit() {

  }

  setData(data: string) {
    this.editorValue = data
  }
}
