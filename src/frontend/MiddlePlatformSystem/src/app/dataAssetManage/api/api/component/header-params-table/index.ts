import {
  Component,
  OnInit,
  Input,
  SimpleChanges,
  ChangeDetectorRef,
  Output,
  EventEmitter,
} from '@angular/core';

@Component({
  selector: 'header-params-table',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class HeaderParamsTableComponent implements OnInit {
  @Output() getHeaderParamsValue = new EventEmitter();
  @Input() headerParams: string;

  tableData: any[] = [];
  headerData: any[] = [];
  selectedColumns: any[] = [];
  constructor(private cdr: ChangeDetectorRef) {}
  ngOnChanges(changes: SimpleChanges): void {
    this.cdr.detectChanges();
    let columnData: any = changes['headerParams'].currentValue;
    if (!columnData) return;
    let arr = JSON.parse(columnData);
    this.tableData = arr.map((item: any, index: number) => {
      item.index = index + 1;
      item.disabled = true;
      return item;
    });
    this.tableData.sort((a: any, b: any) => b.index - a.index);
    this.selectedColumns = this.tableData.filter((item: any) => item.disabled);
  }
  ngOnInit(): void {}
  ngDoCheck(): void {
    this.getHeaderParamsValue.emit(this.selectedColumns);
  }
  onInput(e: any) {
    this.getHeaderParamsValue.emit(this.selectedColumns);
  }
  onAdd() {
    let list: any[] = [];
    list = JSON.parse(JSON.stringify(this.selectedColumns));
    this.selectedColumns = [];
    this.tableData.unshift({ key: '', value: '', index: this.tableData.length + 1 });
    console.log('this.selectedColumns===', this.selectedColumns);
    this.selectedColumns = list;
  }
  onHeaderCheckboxToggle(e: any) {
    console.log('e===', e);
    console.log('this.selectedColumns===', this.selectedColumns);
    this.getHeaderParamsValue.emit(this.selectedColumns);
  }
}
