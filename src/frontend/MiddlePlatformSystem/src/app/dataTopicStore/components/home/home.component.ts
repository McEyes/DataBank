import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import LoginHttpService from 'src/api/common/login';
import { TranslateData } from 'src/app/core/translate/translate-data';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { environment } from 'src/environments/environment';
import { debounceTime, fromEvent } from 'rxjs';

@Component({
  selector: 'app-data-topic-store-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [MessageService, LoginHttpService, ConfirmationService],
})
export class DataTopicStoreHomeComponent implements OnInit {
  @Output() goDetail = new EventEmitter<string>();
  @Output() updateBreadcrumbs = new EventEmitter<string>();

  currMorePanel: string = ''

  newList: any[] = []
  newListTop: any[] = []
  favoriteList: any[] = []
  favoriteListTop: any[] = []
  fastList: any[] = []
  fastListTop: any[] = []
  fileHost: string = environment.FileServer;
  permissionVisible: boolean = false
  constructor(
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly httpService: DataTopicStoreHttpService,
  ) {
    this.getData()
  }

  public async ngOnInit() { }

  async getData() {
    this.getNewData()
    this.getFavoriteData()
    this.getFastData()
  }

  async getNewData() {
    const res = await this.httpService.getTopicRank({
      RankingType: 0,
      Count: 999
    })

    this.newList = res?.data ?? []
    this.newListTop = JSON.parse(JSON.stringify(this.newList))
    this.newList.length = this.newList.length > 10 ? 10 : this.newList.length
  }

  async getFavoriteData() {
    const res = await this.httpService.getTopicRank({
      RankingType: 1,
      Count: 999
    })
    this.favoriteList = res?.data ?? []
    this.favoriteListTop = JSON.parse(JSON.stringify(this.favoriteList))
    this.favoriteList.length = this.favoriteList.length > 10 ? 10 : this.favoriteList.length
  }

  async getFastData() {
    const res = await this.httpService.getTopicRank({
      RankingType: 2,
      Count: 999
    })

    this.fastList = res?.data ?? []
    this.fastListTop = JSON.parse(JSON.stringify(this.fastList))
    this.fastList.length = this.fastList.length > 10 ? 10 : this.fastList.length
  }

  showMore(type: string) {
    this.updateBreadcrumbs.emit()
    this.currMorePanel = type
    if (type === 'new') {
      this.newList = JSON.parse(JSON.stringify(this.newListTop))
    } else if (type === 'favorite') {
      this.favoriteList = JSON.parse(JSON.stringify(this.favoriteListTop))
    } else if (type === 'fastest') {
      this.fastList = JSON.parse(JSON.stringify(this.fastListTop))
    }
  }

  goBack() {
    if (this.currMorePanel === 'new') {
      this.newList.length = this.newList.length > 10 ? 10 : this.newList.length
    } else if (this.currMorePanel === 'favorite') {
      this.favoriteList.length = this.favoriteList.length > 10 ? 10 : this.favoriteList.length
    } else if (this.currMorePanel === 'fastest') {
      this.fastList.length = this.fastList.length > 10 ? 10 : this.fastList.length
    }
    this.currMorePanel = ''
  }

  openDetail(item: any) {
    this.goDetail.emit(item)
  }

  permissionEvent(e: any) {
    e.stopPropagation()
  }

  visibleChange(value: boolean, item: any): void {
    item.permissionVisible = value;
    setTimeout(() => {
      this.initScale()
      this.permissionVisible = true
    }, 50)
  }

  closePermission(item: any) {
    item.permissionVisible = false
    this.permissionVisible = false
  }

  async applyPermission(item: any) {
    const res = await this.httpService.applyPermission({
      topic_id: item.id
    })

    if (res.succeeded) {
      item.permissionVisible = false
      this.permissionVisible = false
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
    } else {
      item.permissionVisible = false
      this.permissionVisible = false
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
  }

  authorVisibleChange(value: boolean, item: any): void {
    item.authorVisible = value;
    setTimeout(() => {
      this.initScale()
      this.permissionVisible = true
    }, 50)
  }

  closeAuthor(item: any) {
    item.authorVisible = false
    this.permissionVisible = false
  }

  async sentEmail(item: any) {
    location.href = 'mailto:' + item.owner_email
    item.authorVisible = false
    this.permissionVisible = false
  }

  initScale() {
    const SWidth = 1920
    const SHeight = 1080

    const width = Number(window.innerWidth)
    const height = Number(window.innerHeight)
    let scale = 1

    if (width / SWidth > height / SHeight) {
      scale = height / SHeight
    } else {
      scale = width / SWidth
    }

    if (scale < 0.75) {
      scale = 0.75
    }

    const scaleEle: any = document.getElementById('business-model-permission-confirm') || {}

    if (!scaleEle?.style) return;
    scaleEle.style.transform = `scale(${scale})`
    scaleEle.style.width = 100 / scale + '%'
    scaleEle.style.height = 100 / scale + '%'
    scaleEle.style.left = -(100 / scale - 100) / 2 + '%'
    scaleEle.style.top = -(100 / scale - 100) / 2 + '%'
  }
}
