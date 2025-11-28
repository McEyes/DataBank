import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as echarts from 'echarts';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';


@Component({
  selector: 'app-function-api-report',
  templateUrl: './function-api-report.component.html',
  styleUrls: ['./function-api-report.component.scss'],
  providers: [CommonHttpService, DictHttpService]
})
export class FunctionApiReportComponent implements OnInit {

  currentLanguage: any = 'en';
  eApiChart: any = null
  eApiChartOption: any = {
    color: ['#188df0', '#a3fb01ff'],
    title: {
      show: false
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    legend: {},
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    yAxis: [{
      type: 'value',
      name: 'Quality Score',
      min: 0,
      max: 100,
      interval: 10,
      axisLabel: {
        formatter: '{value} 分'
      },
      // boundaryGap: [0, 0.01]
    }, {
      type: 'value',
      name: 'Data Table Quantity',
      min: 0,
      // max: 150,
      // interval: 15,
      axisLabel: {
        formatter: '{value} 个'
      },
      // boundaryGap: [0, 0.01]
    }
    ],
    xAxis: {
      type: 'category',
      data: ['HR', 'Admin&Common Service', 'EHS', 'ME', 'Digital Factory', 'Material'],
      axisLabel: {
        interval: 0,
        rotate: 40,
        width: 100,
        overflow: "truncate",
        ellipsis: "..."
      }
    },
    series: [
      {
        name: 'Data Table Quantity',
        type: 'bar',
        barMaxWidth: '30',
        yAxisIndex: 1,
        tooltip: {
          valueFormatter: function (value: any) {
            return value + ' 个';
          }
        },
        data: [244, 237, 237, 163, 124, 107],
        showBackground: false,
        itemStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: '#83bff6' },
            { offset: 0.5, color: '#188df0' },
            { offset: 1, color: '#188df0' }
          ])
        },
      },
      {
        name: 'Quality Score',
        type: 'line',
        barMaxWidth: '30',
        yAxisIndex: 0,
        tooltip: {
          valueFormatter: function (value: any) {
            return value + ' 分';
          }
        },
        data: [90, 88, 88, 84, 84, 85],
        markLine: {
          silent: true,
          lineStyle: {
            color: '#333'
          },
          data: [
            {
              yAxis: 85
            }
          ]
        }
      }
    ],
    visualMap: [{
      top: 10,
      right: 50,
      show: false,
      seriesIndex: 1,
      pieces: [
        {
          gt: 0,
          lte: 85,
          color: '#a3fb01ff'//
        },
        // {
        //   gt: 0,
        //   lte: 85,
        //   color: '#e4d615ff'
        // },
        {
          gt: 85,
          lte: 1000,
          color: '#10da09ff'//绿色
        }
      ],
      // outOfRange: {
      //   color: '#999'
      // }
    }],
  };

  apiMoreThan85: number = 0;
  apiLessThan85: number = 0;
  dataCategoryTable: any[] = [];
  functionTable: any[] = [];
  dataCategoryList: any[] = [];

  constructor(
    private translate: TranslateService,
    private dictService: DictHttpService,
    private commonHttp: CommonHttpService) { }

  ngOnInit(): void {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getEChartData();
    this.getScopeData();
  }

  getScopeData() {
    //统计质量分数大于等于85的api数量和小于85的api数量,
    // 并根据部门进行分组
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery",
      `select owner_depart as FunctionName,
case when data_category is null then 'Other' else data_category end as DataCategory,
case when owner_depart is not null then 'function' else owner_depart end as datatype,
SUM(case when quality_score>=85 then 1 else 0 end) ApiMoreThan85,
SUM(case when quality_score>=85 then 0 else 1 end) ApiLessThan85
from metadata_table_view
where owner_depart is not null and owner_depart <>'' and status =1 and ctl_status =1
group by owner_depart,data_category
UNION ALL
select '' as FunctionName,
case when data_category is null then 'Other' else data_category end as DataCategory,
'data_category' as datatype,
SUM(case when quality_score>=85 then 1 else 0 end) ApiMoreThan85,
SUM(case when quality_score>=85 then 0 else 1 end) ApiLessThan85
from metadata_table_view
where owner_depart is not null and owner_depart <>'' and status =1 and ctl_status =1
group by data_category
        `).then((res: any) => {
        if (res?.data?.data) {
          //先根据functionname排序，再根据 datacategory 排序，最后根据apimorethan85数量有高到低排序,
          // 相同functionname的datacategory按数量从高到低排序,
          // 相同datacategory的functionname按functionname排序
          // 相同的functionname，第一个统计的datacategory数量,后面的datacategory数量为0
          this.functionTable = res?.data?.data.filter((item: any) => item.datatype === 'function').sort((a: any, b: any) => {
            a.key = a.functionname + a.datacategory;
            if (a.functionname === b.functionname) {
              return b.apimorethan85 - a.apimorethan85;
              // if (a.datacategory === b.datacategory) {
              //   return b.apimorethan85 - a.apimorethan85;
              // }
              // return a.datacategory.localeCompare(b.datacategory);
            }
            return a.functionname.localeCompare(b.functionname);
          });
          // var lastitem = {functionname:'', rowspan: 0 };
          // this.functionTable.forEach((item: any) => {
          //   if (lastitem?.functionname === item.functionname) {
          //     lastitem.rowspan ++;
          //   } else {
          //     lastitem = item;
          //     lastitem.rowspan = 1;
          //   }
          // });


          //根据apimorethan85数量有高到低排序
          this.dataCategoryTable = res?.data?.data.filter((item: any) => item.datatype === 'data_category').sort((a: any, b: any) => b.apimorethan85 - a.apimorethan85);
          this.apiMoreThan85 = this.functionTable.reduce((pre: number, cur: any) => pre + cur.apimorethan85, 0);
          this.apiLessThan85 = this.functionTable.reduce((pre: number, cur: any) => pre + cur.apilessthan85, 0);
          this.animateCount('apiMoreThan85', this.apiMoreThan85);
          this.animateCount('apiLessThan85', this.apiLessThan85);
        }
      });

    // this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery",
    //   `select SUM(case when quality_score>=85 then 1 else 0 end) ApiMoreThan85,
    //     SUM(case when quality_score>=85 then 0 else 1 end) ApiLessThan85
    //     from metadata_table_view
    //     where owner_depart is not null and owner_depart <>'' and status =1 and ctl_status =1`).then((res: any) => {
    //     if (res?.data?.data) {
    //       this.apiMoreThan85 = res?.data?.data[0].apimorethan85;
    //       this.apiLessThan85 = res?.data?.data[0].apilessthan85;
    //     }
    //   });
  }

  getEChartData() {
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery",
      `select owner_depart as FunctionName,count(table_name) ApiAmount,SUM(quality_score)/count(table_name) quality_score
        from metadata_table_view
        where owner_depart is not null and owner_depart <>'' and status =1 and ctl_status =1
        group by owner_depart`).then((res: any) => {
        if (res?.data?.data) {
          //res?.data?.data 根据 apiamount 排序
          var dataList = res?.data?.data.sort((a: any, b: any) => a.apiamount - b.apiamount);
          this.eApiChartOption.series[0].data = dataList.map((item: any) => item.apiamount);
          this.eApiChartOption.series[1].data = dataList.map((item: any) => item.quality_score);
          this.eApiChartOption.xAxis.data = dataList.map((item: any) => item.functionname);
          this.eApiChartOption.yAxis[1].max = Math.ceil(Math.max(...this.eApiChartOption.series[0].data) / 10) * 10;
          // this.eApiChartOption.yAxis[1].max = Math.ceil(Math.ceil(Math.max(...this.eApiChartOption.series[0].data) / 10)/5) * 50;
          //max 计算返回5的倍数
          // this.eApiChartOption.yAxis[1].max = Math.ceil(this.eApiChartOption.yAxis[1].max / 5) * 5;
          this.eApiChartOption.yAxis[1].interval = Math.round(this.eApiChartOption.yAxis[1].max / 10);
          // this.eApiChart.setOption(this.eApiChartOption);
          this.initEChart();
        }
      });
  }


  initEChart() {
    let eApiDom = document.getElementById('function-api-chart') as HTMLDivElement;
    if (eApiDom) {
      if (!this.eApiChart) this.eApiChart = echarts.init(eApiDom);
      this.eApiChart.setOption(this.eApiChartOption);
    }
  }
  animateCount(target: 'apiMoreThan85' | 'apiLessThan85', finalValue: number) {
    let start = 0;
    const duration = 1000; // 1 second for animation
    const range = finalValue - start;
    let startTime: number | null = null;

    const step = (timestamp: number) => {
      if (!startTime) startTime = timestamp;
      const progress = timestamp - startTime;
      const percentage = Math.min(progress / duration, 1);
      const currentValue = Math.floor(start + range * percentage);
      this[target] = currentValue;

      if (progress < duration) {
        window.requestAnimationFrame(step);
      } else {
        this[target] = finalValue; // Ensure it ends on the exact value
      }
    };

    window.requestAnimationFrame(step);
  }

  async getDBTypeList() {
    let res = await this.dictService.codes('data_category');
    if (res.success) {
      this.dataCategoryList = res.data.filter((f: any) => f.dictCode == 'data_category');
      // this.dataCategoryList.splice(0,0,{itemText:'All',itemValue:'',dictCode:'selectAll'})
      this.dataCategoryList.forEach((f: any) => {
        this.translate.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
        });
      });
    }
  }
}
