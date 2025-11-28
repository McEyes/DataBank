import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import LoginHttpService from 'src/api/common/login';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-topic-edit-step2',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, LoginHttpService, CommonHttpService],
})
export class DataTopicEditStep2Component implements OnInit {

  isView: boolean = false
  invalid: boolean = false
  formData: any = {
    "dataSourceDefinition": {
      "process_time": "",
      "pic_ntid": "",
      "pic": ""
    },
    "dataSourceIngestion": {
      "process_time": "",
      "pic_ntid": "",
      "pic": ""
    },
    "physicalModeling": {
      "process_time": "",
      "pic_ntid": "",
      "pic": ""
    },
    "tScriptGeneration": {
      "process_time": "",
      "pic_ntid": "",
      "pic": ""
    },
    "apiGeneration": {
      "process_time": "",
      "pic_ntid": "",
      "pic": ""
    }
  }

  inputSetting: any = {
    parameters: []
  }

  outputSetting: any = {
    type: "array",
    isPaged: true,
    parameters: []
  }
  currLang: string = 'en'
  userList: any[] = []
  dataTypeList: any[] = [
    {name: 'String', id: 'string'},
    {name: 'Number', id: 'number'},
    {name: 'Integer', id: 'integer'},
    {name: 'Boolean', id: 'boolean'},
    {name: 'Array', id: 'array'},
  ]
  formatObj: any = {
    string: [
      {name: 'Text', id: 'text'},
      {name: 'Date', id: 'date'},
      {name: 'Datetime', id: 'date-time'},
      {name: 'Email', id: 'email'},
      {name: 'UUID', id: 'uuid'},
      {name: 'URI', id: 'uri'}
    ],
    integer: [
      {name: 'Int32', id: 'int32'},
      {name: 'Int64', id: 'int64'},
    ],
    number: [
      {name: 'Float', id: 'float'},
      {name: 'Double', id: 'double'},
    ],
    array: [
      {name: 'Float', id: 'float'},
      {name: 'Double', id: 'double'},
      {name: 'Int32', id: 'int32'},
      {name: 'Int64', id: 'int64'},
      {name: 'String', id: 'string'},
    ],
    boolean: [],
  }
  outTypeList: any[] = [{name: 'Array', id: 'array'}, {name: 'Object', id: 'object'}]
  isList: any[] = [{name: TranslateData.yes, id: true}, {name: TranslateData.no, id: false}]
  constructor(
    public readonly httpService: LoginHttpService,
    private readonly messageService: MessageService,
    private readonly commonHttp: CommonHttpService,
  ) {
    this.getUserList()
    this.currLang = localStorage.getItem('lang') ?? 'en'
  }

  public ngOnInit() {
    console.log(this.inputSetting.parameters)
    console.log(this.outputSetting.parameters)
  }

  // 用户列表d
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.commonHttp.getUserList().then((res: any) => {
        let arr = res.data;
        const uniqueArr = Array.from(
          new Map(arr.map((item: any) => [item['id'], item])).values()
        );
        this.userList = uniqueArr;
        sessionStorage.setItem('userList', JSON.stringify(this.userList));
      });
    }
  }

  changePIC(e: any, formData: any) {
    const data = this.userList.find((item: any) => {
      return item.id === e.value
    })

    formData.pic = data.name
  }

  addInputData() {
    this.inputSetting.parameters.unshift({
      "name": "",
      "required": true,
      "dataType": "",
      "description": "",
      "format": ""
    })
  }

  addOutputData() {
    this.outputSetting.parameters.unshift({
      "name": "",
      "dataType": "",
      "format": "",
      "description": ""
    })
  }

  deleteOutputData(i: number) {
    this.outputSetting.parameters.splice(i, 1)
  }

  deleteInputData(i: number) {
    this.inputSetting.parameters.splice(i, 1)
  }

  changeDataType(item:any) {
    item.format = ''
  }
}
