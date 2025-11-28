import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'level-one-panel',
  templateUrl: './index.html',
  styleUrls: ['./index.scss']
})
export class LevelOnePanelComponent implements OnInit {
  @Input() height: string = 'calc(100% - 65px)'
  @Input() titleName: string = 'defult'
  @Input() innerHeight: string;
  @Input() isBottom: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

}
