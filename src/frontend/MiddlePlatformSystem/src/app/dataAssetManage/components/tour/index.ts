import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';

@Component({
  selector: 'system-tour',
  templateUrl: './index.html',
  styleUrls: ['./index.scss']
})
export class SystemTourComponent implements OnInit {
  @Input() height: string = 'calc(100% - 65px)'
  @Input() titleName: string = 'defult'
  @Input() innerHeight: string;
  @Input() isBottom: boolean = false;
  @Output() close = new EventEmitter<string>();
  @Output() step1Click = new EventEmitter<string>();

  showStatus: string = ''
  lang: string = ''
  constructor() {
    this.lang = localStorage.getItem('lang') ?? 'en'
   }

  ngOnInit(): void {
  }

  closePanel() {
    this.close.emit(this.showStatus)
  }

  step1ClickEvent() {
    this.step1Click.emit()
  }
}
