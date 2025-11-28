import { Component, OnInit,Input,SimpleChanges,ChangeDetectorRef,
  Output,
  EventEmitter,
} from '@angular/core';

@Component({
  selector: 'paramsTableComp',
  templateUrl: './index.html',
  styleUrls: ['./index.scss']
})
export class ParamsTableCompComponent implements OnInit {
  @Input()  products: any[] =[]
  @Output() paramsTable = new EventEmitter();
  tableData: any[];
  constructor( private cdr:ChangeDetectorRef) { }
  ngOnChanges(changes: SimpleChanges): void {
    this.cdr.detectChanges();
    const columnData = changes['products'].currentValue;
    columnData && columnData.forEach((element: any, index: number)=> element.index = index + 1);
    this.tableData=columnData
  }
  ngOnInit(): void {
  }
  onBlur(e: any) {
    this.paramsTable.emit(this.tableData);
  }
}
