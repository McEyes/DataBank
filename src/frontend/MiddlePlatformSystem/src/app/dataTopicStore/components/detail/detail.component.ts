import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TranslateData } from 'src/app/core/translate/translate-data';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-data-topic-store-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss'],
  providers: [MessageService, DataTopicStoreHttpService, ConfirmationService],
})
export class DataTopicStoreDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  rating: number = 5
  five: number = 5
  four: number = 4
  three: number = 3
  two: number = 2
  one: number = 1

  localhost: string = ''
  formData: any = {
    star_ratio: {
      1: 0,
      2: 0,
      3: 0,
      4: 0,
      5: 0,
    }
  }
  commentList: any[] = []
  favoriteList: any[] = []
  evaluate: number = 0
  commentContent: string = ''
  loading: boolean = false
  fileHost: string = environment.FileServer;
  currPanel: string = 'detail'
  swiperEl: any = null;
  currTitleIndex: number = 0
  datalineageUrl: any = ''
  hidde: boolean = false
  permissionPopVisible: boolean = false
  permissionVisible: boolean = false

  ratingsVisible: boolean = false
  currtentId: string = ''
  jsonFormulaArray: any[] = []
  goalData: any = []
  constructor(
    private readonly httpService: DataTopicStoreHttpService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    private readonly domSanitizer: DomSanitizer,
  ) {
    this.localhost = location.origin
    this.getFavoriteData()
  }

  public async ngOnInit() {
    if (this.field?.id) {
      this.currtentId = this.field?.id
      await this.getData()
      await this.getCommentData()
      // this.initSwiper()
    }
  }

  async getData() {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.getTopicDetail(this.currtentId)

    if (res.succeeded) {
      this.formData = res.data
      this.goalData = res?.extras?.goals
      this.formData.ratingsInt = parseInt(this.formData.ratings)
      if (this.formData.tags) {
        const tags = this.formData.tags.split(',')
        this.formData.tags = []
        tags.forEach((item: any) => {
          this.formData.tags.push(item?.trim())
        })
      } else {
        this.formData.tags = []
      }

      this.datalineageUrl = res?.data?.datalineage_url ? this.domSanitizer.bypassSecurityTrustResourceUrl(res?.data?.datalineage_url) : ''

      this.jsonFormulaArray = this.buildJsonFormula(this.formData.json_formula)
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });
    }

    this.loading = false
  }

  buildJsonFormula(inputString: string) {
    if (!inputString) {
      return []
    }
    // 定义运算符集合
    const operators = ['(', ')', '*', '/', '+', '-', '=', '%'];

    // 使用正则表达式按运算符分割字符串，同时保留分隔符
    const tokens = inputString.split(/([*\/+\-=%()])/).filter(token => token.trim() !== '');

    // 将分割结果转换为对象数组
    const resultArray = tokens.map(token => {
      let type = 'unknown';

      // 判断token类型
      if (operators.includes(token)) {
        type = 'operator';
      } else if (!isNaN(parseFloat(token)) && isFinite(Number(token))) {
        type = 'number';
      } else {
        type = 'string';
      }

      return {
        value: token,
        type: type
      };
    });

    return resultArray;
  }

  async getCommentData() {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.getCommentList({
      topic_id: this.currtentId,
      pageIndex: 1,
      pageSize: 99,
    })

    if (res.succeeded) {
      this.commentList = res?.data?.data
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });
    }
    this.loading = false
  }

  async createComment() {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.createComment({
      topic_id: this.currtentId,
      content: this.commentContent
    })

    if (res.succeeded) {
      this.commentContent = ''
      this.loading = false
      this.getCommentData()
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });
    }
    this.loading = false
  }

  async setCommentLike(item: any, like: boolean) {
    if (this.loading) return
    this.loading = true

    let likeType = 0
    // 取消喜欢
    if (item.is_liked === true && like === true) {
      likeType = 2
      item.is_liked = null
      --item.liked_count
      // 取消不喜欢
    } else if (item.is_liked === false && like === false) {
      likeType = 3
      item.is_liked = null
      --item.disliked_count
    } else if (like === false) {
      likeType = 1
      // 之前喜欢
      if (item.is_liked) {
        --item.liked_count
      }
      ++item.disliked_count
      item.is_liked = false
    } else {
      // 之前不喜欢
      if (item.is_liked === false) {
        --item.disliked_count
      }
      ++item.liked_count
      item.is_liked = true
    }

    const res = await this.httpService.setCommentLike({
      comments_id: item.id,
      like_type: likeType // like 0; dislike 1; cancellike 2; canceldislike 3
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });
    }

    this.loading = false
  }

  async setFavorite() {
    if (this.loading) return
    this.loading = true

    const res = await this.httpService.favorite({
      topic_id: this.currtentId
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
      this.formData.is_favorite = !this.formData.is_favorite
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }

    this.loading = false
  }

  permissionEvent() {
    this.confirmationService.confirm({
      message: TranslateData.askPermissionTip,
      header: TranslateData.confirm,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: TranslateData.yes,
      rejectLabel: TranslateData.no,
      accept: async () => {
        this.applyPermission()
      },
    });
  }

  async applyPermissionRequest() {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.applyPermission({
      topic_id: this.currtentId
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
    this.loading = false
  }

  async userEvaluate() {
    if (this.loading) return
    this.loading = true

    const res = await this.httpService.rating({
      topic_id: this.currtentId,
      star: this.evaluate  // 0 like
    })

    this.loading = false
    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
      this.getData()
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }

  }

  async selectPrase(type: boolean) {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.like({
      topic_id: this.currtentId,
      like_type: type ? 0 : 1  // 0 like
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
      this.formData.is_liked = type
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
    this.loading = false
  }

  copyText(text: string): void {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    this.messageService.add({
      severity: 'success',
      summary: TranslateData.success,
      detail: TranslateData.copy,
    });
  }

  dataPreview() {
    if (!this.formData.has_datapreview_permission) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.previewPermissions,
      });

      return
    }

    this.currPanel = 'preview'
  }

  goDetail() {
    this.hidde = false
    this.currPanel = 'detail'
  }

  dataLineage() {
    this.hidde = true
    setTimeout(() => {
      this.currPanel = 'dataLineage'
    }, 50)
  }

  initSwiper() {
    const swiperEl: any = document.getElementById('case-info-swiper');

    const param = {
      id: 'case-info'
    }

    swiperEl && Object.assign(swiperEl, param);

    swiperEl?.addEventListener('swiperslidechange', (event: any) => {
      if (event.target.id === 'case-info') {
        // this.currTitleIndex = event?.detail[0]?.realIndex || 0
      }
    });

    swiperEl && swiperEl.initialize();

    this.swiperEl = swiperEl
  }

  switchPic(type: number) {
    if (this.currTitleIndex === 4 && type > 0) {
      this.currTitleIndex = 0
      this.swiperEl.swiper.slideToLoop(0)
      return
    }
    if (this.currTitleIndex === 0 && type < 0) {
      this.currTitleIndex = 4
      this.swiperEl.swiper.slideToLoop(4)
      return
    }
    this.currTitleIndex += type
    this.swiperEl.swiper.slideToLoop(this.currTitleIndex)
  }

  visibleChange(value: boolean): void {
    this.permissionPopVisible = value;
    setTimeout(() => {
      this.initScale()
      this.permissionVisible = true
    }, 50)
  }

  closePermission() {
    this.permissionVisible = false
    this.permissionPopVisible = false
  }

  async applyPermission() {
    const res = await this.httpService.applyPermission({
      topic_id: this.currtentId
    })

    if (res.succeeded) {
      this.permissionVisible = false
      this.permissionPopVisible = false
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
    } else {
      this.permissionVisible = false
      this.permissionPopVisible = false
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
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

    scaleEle.style.transform = `scale(${scale})`
    scaleEle.style.width = 100 / scale + '%'
    scaleEle.style.height = 100 / scale + '%'
    scaleEle.style.left = -(100 / scale - 100) / 2 + '%'
    scaleEle.style.top = -(100 / scale - 100) / 2 + '%'
  }

  ratingsVisibleChange(e: any) {
    if (e) {
      setTimeout(() => {
        this.initScale()
      }, 50)

    }
  }

  async getFavoriteData() {
    const res = await this.httpService.getMyFavorite({})
    this.favoriteList = res?.data ?? []
  }

  async changeDetail(item: any) {
    this.currtentId = item.id
    this.hidde = true
    await this.getData()
    await this.getCommentData()
    this.hidde = false
  }

  getRatings() {
    return parseInt(this.formData?.ratings || 0)
  }
}
