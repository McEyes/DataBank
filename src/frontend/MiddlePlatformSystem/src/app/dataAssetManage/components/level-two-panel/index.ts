import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'level-two-panel',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class LevelTwoPanelComponent implements OnInit {
  @Input() height: string = 'null';
  @Input() titleName: string = 'defult';
  @Input() isValid: boolean = false;

  constructor() {}

  ngOnInit(): void {}
}
