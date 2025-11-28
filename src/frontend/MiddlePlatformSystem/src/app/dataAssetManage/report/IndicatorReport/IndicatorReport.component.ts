import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as echarts from 'echarts';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';
import * as XLSX from 'xlsx';


@Component({
  selector: 'IndicatorReport',
  templateUrl: './IndicatorReport.component.html',
  styleUrls: ['./IndicatorReport.component.scss'],
  providers: [CommonHttpService, DictHttpService]
})
export class IndicatorReportComponent implements OnInit {
  colorList: any = [];
  colorLogList: any = [];
  stockList: any = [];
  logList: any = [];
  indicatorFunctionList: any = [];
  currentLanguage: any = 'en';
  title: any = '';
  eApiColumnarChart: any = null
  eApiLogColumnarChart: any = null
  eApiFunctionChart: any = null
  private highlightedIndex: number = -1;
  loading: boolean = false;
  eChartFunctionOption: any = {
    title: {
      text: 'Indicator Quantity',
      left: 'center'
    },
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        const percentage = ((params.value / this.totalIndicator) * 100).toFixed(2) + '%';
        return `Total: ${this.totalIndicator}<br/>${params.name}: ${params.value} (${percentage})`;
      }
    },

    series: [
      {
        name: "COME indicators Quantity",
        type: 'pie',
        radius: '70%', // 饼图半径（百分比或像素）
        data: [
        ],
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        },
        label: {
          formatter: '{b}: {d}%' // 显示百分比
        }
      }
    ]
  };
  totalIndicator: number = 0;
  apiMoreThan85: number = 0;
  apiLessThan85: number = 0;
  indicator: number = 0;
  noIndicator: number = 0;
  eApiColumnarChartOption: any = {
    title: {
      text: '',
      left: 'center'
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    xAxis: [
      {
        type: 'category',
        data: [],
        axisLabel: {
          interval: 0,
          rotate: 30 // 如果名称太长可以旋转
        }
      }
    ],
    yAxis: [
      {
        type: 'value',
        name: 'Count',
      },
      {
        type: 'value',
        name: 'Score',
        min: 0,
        max: 1,
      }


    ],
    series: [
    ]
  };
  eApiColumnarChartLogOption: any = {
    title: {
      text: "",
      subtext: '', // 设置副标题
      left: 'center'
    },

    toolbox: {
      feature: {
        // 自定义导出数据按钮
        myCustomExport: {
          show: true,
          title: 'ExportData', // 自定义按钮的提示文字
          icon: 'path://M4.7,22.9L29.3,45.5L54.7,23.4M4.6,43.6L4.6,58L53.8,58L53.8,43.6M29.2,45.1L29.2,0', // 可以自定义图标，这里使用一个占位符
          onclick: () => {
            this.exportExcel();
          }
        }
      }
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    xAxis: [
      {
        type: 'category',
        data: [],
        axisLabel: {
          interval: 0,
          rotate: 30 // 如果名称太长可以旋转
        }
      }
    ],
    yAxis: [
      {
        type: 'value',
        name: 'Visited Times',

      },
      {
        name: 'Indicator Quantity',
        type: 'value'
      }
    ],
    series: [
    ]
  };
  constructor(
    private translate: TranslateService,
    private dictService: DictHttpService,
    private commonHttp: CommonHttpService) { }

  ngOnInit(): void {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getIndicatorGroupByFunctionPieData();
    this.getColumnarEChartData(null);

    this.getLogColumnarEChartData();
  }

  exportExcel() {
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/app_source_api_visied_view/sqlQuery",
      `select app_source,indicator_quantity,visited_indicator_quantity,indicator_times  from app_source_api_visied_view  order by indicator_times desc`).then((res: any) => {
        if (res?.data?.data) {
          let dataCategoryTable = res?.data?.data;
          let totalquantity = dataCategoryTable.reduce((total: number, item: any) => total + item.indicator_quantity, 0);
          let rptData: any[] = [];
          res?.data?.data.forEach((item: any) => {
            rptData.push({
              'App': item.app_source,
              'Total Visited Indicator Quantity': totalquantity,
              'Indicator Quantity': item.indicator_quantity,
              'Visited Indicator Quantity': item.visited_indicator_quantity,
              'Indicator Visited Times': item.indicator_times
            });
          });
          this.exportToExcel(rptData, "Indicator Visits in the Last 30 Days");
        }
      });
  }

  getLogColumnarEChartData() {
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/app_source_api_visied_view/sqlQuery",
      `select app_source,indicator_quantity,visited_indicator_quantity,indicator_times  from app_source_api_visied_view  order by indicator_times desc `, 1, 10).then((res: any) => {
        if (res?.data?.data) {
          let dataCategoryTable = res?.data?.data;
          let totalquantity = dataCategoryTable.reduce((total: number, item: any) => total + item.indicator_quantity, 0);
          let legendALL: number[] = [];
          let indicator_quantityList: number[] = [];
          let visited_indicator_quantityList: number[] = [];
          let indicator_timesList: number[] = [];

          dataCategoryTable.forEach((e: any) => {
            legendALL.push(e.app_source);
            indicator_quantityList.push(e.indicator_quantity);
            visited_indicator_quantityList.push(e.visited_indicator_quantity);
            indicator_timesList.push(e.indicator_times);
          })

          let indicator_quantityModel = {
            name: 'App Indicator Quantity',
            type: 'bar',
            stack: 'Ad',
            yAxisIndex: 1,
            data: indicator_quantityList
          }

          let visited_indicator_quantityModel = {
            name: 'Visited Indicator Quantity',
            type: 'bar',
            stack: 'Ad',
            yAxisIndex: 1,
            data: visited_indicator_quantityList
          }

          let indicator_timesModel = {
            name: 'Indicator Visited Times',
            type: 'bar',
            yAxisIndex: 0,
            data: indicator_timesList
          }
          this.logList.push(indicator_quantityModel);
          this.logList.push(visited_indicator_quantityModel);
          this.logList.push(indicator_timesModel);

          this.eApiColumnarChartLogOption.xAxis[0].data = legendALL;
          this.eApiColumnarChartLogOption.series = this.logList;
          this.eApiColumnarChartLogOption.title.text = "Top 10 Indicator Visits in the Last 30 Days";
          this.eApiColumnarChartLogOption.title.subtext = "Total Visited Indicator Quantity："+totalquantity;
          this.initLogColumnarEChart();
        }
      });
  }

  initLogColumnarEChart() {
    let eApiDom = document.getElementById('indicatorapp') as HTMLDivElement;
    if (eApiDom) {
      if (!this.eApiLogColumnarChart) this.eApiLogColumnarChart = echarts.init(eApiDom);
      this.eApiLogColumnarChart.setOption(this.eApiColumnarChartLogOption);
    }
  }

  // 导出数据为 Excel 文件
  exportToExcel(data: any[], fileName: string) {
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    XLSX.writeFile(wb, `${fileName}.xlsx`);
  }

  initColumnarEChart() {
    let eApiDom = document.getElementById('function-api-chart') as HTMLDivElement;
    if (eApiDom) {
      if (!this.eApiColumnarChart) this.eApiColumnarChart = echarts.init(eApiDom);
      this.eApiColumnarChart.setOption(this.eApiColumnarChartOption);
    }
  }
  // 生成随机颜色的方法
  getRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
  initFunctionEChart() {
    let eApiDom = document.getElementById('indicatorFunctionPieChart') as HTMLDivElement;
    this.eChartFunctionOption.series[0].data = this.indicatorFunctionList;
    if (eApiDom) {
      if (!this.eApiFunctionChart) this.eApiFunctionChart = echarts.init(eApiDom);
      this.eApiFunctionChart.setOption(this.eChartFunctionOption);
      this.eApiFunctionChart.on('click', (params: any) => {
        const seriesIndex = params.seriesIndex;
        const dataIndex = params.dataIndex;
        //说明二次点击，那么取消选中以及高亮
        if (dataIndex == this.highlightedIndex && this.highlightedIndex != -1) {
          this.eApiFunctionChart.dispatchAction({
            type: 'downplay',
            seriesIndex: seriesIndex,
            dataIndex: this.highlightedIndex
          });
          this.getColumnarEChartData(null);
          this.highlightedIndex = -1;
        }
        else {
          // 取消之前高亮的扇形
          if (this.highlightedIndex !== -1) {
            this.eApiFunctionChart.dispatchAction({
              type: 'downplay',
              seriesIndex: seriesIndex,
              dataIndex: this.highlightedIndex
            });
          }

          // 高亮当前点击的扇形
          this.eApiFunctionChart.dispatchAction({
            type: 'highlight',
            seriesIndex: seriesIndex,
            dataIndex: dataIndex
          });

          // 更新当前高亮的扇形索引
          this.highlightedIndex = dataIndex;

          this.getColumnarEChartData(params.data.name);
        }

      })
    }
  }
  getColumnarEChartData(department: any) {

    let sql = " select * from ( select owner_depart,isincrease,AVG(quality_score),count(1) from metadata_indicator_score_view  where ctl_status =1 ";
    if (department) {
      sql += "  and owner_depart='" + department + "'";
    }
    sql += "group by  owner_depart ,isincrease  order  by owner_depart   ) aa "

    sql += " union all select * from ( select  owner_depart, false isincrease,AVG(quality_score) avg,-1 count from metadata_indicator_score_view  where ctl_status =1";
    if (department) {
      sql += "  and owner_depart='" + department + "'";
    }
    sql += " group  by owner_depart) bb ";
    this.stockList = [];
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_indicator_score_view/sqlQuery",
      sql).then((res: any) => {
        if (res?.data?.data) {
          let dataCategoryTable = res?.data?.data;
          let legendALL = Array.from(new Set(dataCategoryTable.map((item: any) => item.owner_depart)));
          let arrStockList: number[] = [];
          let arrindicatorList: number[] = [];

          let arrIndicatorScore: number[] = [];
          let arrIncrementQualityScore: number[] = [];
          let arrStockQualityScore: number[] = [];

          let arrNonStockList: number[] = [];
          legendALL.forEach((e: any) => {
            let Stock = dataCategoryTable.filter((item: any) => item.owner_depart == e && item.isincrease == true && item.count != -1);
            let NonStock = dataCategoryTable.filter((item: any) => item.owner_depart == e && item.isincrease == false && item.count != -1);
            let Indicator = dataCategoryTable.filter((item: any) => item.owner_depart == e && item.isincrease == false && item.count == -1);
            if (Indicator && Indicator[0]) {
              arrIndicatorScore.push(Indicator[0].avg.toFixed(2));
            }
            else {
              arrIndicatorScore.push(0);
            }
            let total = 0;
            if (Stock && Stock[0]) {
              arrStockList.push(Stock[0].count);
              arrStockQualityScore.push(Stock[0].avg.toFixed(2))
              total += Stock[0].count;
            }
            else {
              arrStockList.push(0);
              arrStockQualityScore.push(0);
            }
            if (NonStock && NonStock[0]) {
              arrNonStockList.push(NonStock[0].count);
              arrIncrementQualityScore.push(NonStock[0].avg.toFixed(2))
              total += NonStock[0].count;
            }
            else {
              arrNonStockList.push(0);
              arrIncrementQualityScore.push(0)
            }

            arrindicatorList.push(total);

          })
          let indicatorModel = {
            name: 'Indicator Count',
            type: 'bar',
            yAxisIndex: 0,
            emphasis: {
              focus: 'series'
            },
            data: arrindicatorList
          }

          let StockModel = {
            name: 'Stock',
            type: 'bar',
            stack: 'Ad',
            data: arrStockList
          }
          let NonStockModel = {
            name: 'Increment',
            type: 'bar',
            stack: 'Ad',
            data: arrNonStockList
          }
          let QualityScoreStockModel = {
            name: 'Stock Score',
            type: 'line',
            yAxisIndex: 1,
            data: arrStockQualityScore
          }
          let QualityScoreIncrementModel = {
            name: 'Increment Score',
            type: 'line',
            yAxisIndex: 1,
            data: arrIncrementQualityScore
          }
          let IndicatorScoreModel = {
            name: 'Indicator Score',
            type: 'line',
            yAxisIndex: 1,
            data: arrIndicatorScore
          }
          this.stockList.push(indicatorModel);
          this.stockList.push(StockModel);
          this.stockList.push(NonStockModel);
          this.stockList.push(IndicatorScoreModel);
          this.stockList.push(QualityScoreStockModel);
          this.stockList.push(QualityScoreIncrementModel);
          this.eApiColumnarChartOption.xAxis[0].data = legendALL;
          this.eApiColumnarChartOption.series = this.stockList;
          var totalSql = " select  case isincrease when true then '存量' else '增量' end type,AVG(quality_score) from metadata_indicator_score_view  where ctl_status =1 ";
          if (department) {
            totalSql += "  and owner_depart='" + department + "'";
          }
          totalSql += " group by   isincrease   union all select  '总量', AVG(quality_score) from metadata_indicator_score_view  where ctl_status =1  ";
          if (department) {
            totalSql += "  and owner_depart='" + department + "'";
          }
          //获取增量 增量  存量
          this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_indicator_score_view/sqlQuery", totalSql).then((res: any) => {
            if (res?.data?.data) {
              let zong = res?.data?.data.filter((item: any) => item.type == '总量');
              let cun = res?.data?.data.filter((item: any) => item.type == '存量');
              let zeng = res?.data?.data.filter((item: any) => item.type == '增量')

              let tx = "Indicator Score：";
              if (zong.length > 0) {
                tx += zong[0]?.avg.toFixed(2);
              } else {
                tx += "0"
              }
              tx += "/Stock Score："
              if (cun.length > 0) {
                tx += cun[0]?.avg.toFixed(2);
              } else {
                tx += "0"
              }
              tx += "/Increment Score："
              if (zeng.length > 0) {
                tx += zeng[0]?.avg.toFixed(2);
              } else {
                tx += "0"
              }
              this.eApiColumnarChartOption.title.text = tx;
              this.initColumnarEChart();
            }
          })


        }
      });
  }

  getIndicatorGroupByFunctionPieData() {
    this.commonHttp.CallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_isindicator_view/sqlQuery",
      `	 SELECT  owner_depart,  SUM(case when isindicator then 1 else 0 end)  isindicatorSum,
        COUNT(isindicator) FROM (select  distinct column_id,owner_depart,isindicator,isdashboard
        from metadata_table_isindicator_view where  ctl_status =1   ) tmp
        where owner_depart<>''  group by   owner_depart`).then((res: any) => {
        if (res?.data?.data) {
          res?.data?.data.forEach((soure: any) => {

            if (soure.isindicatorsum > 0) {
              var m = {
                value: soure.isindicatorsum,
                name: soure.owner_depart
              }
              this.indicatorFunctionList.push(m);

            }

          })
          this.totalIndicator = res?.data?.data.reduce((sum: any, item: any) => sum + item.isindicatorsum, 0);
          this.initFunctionEChart();
        }
      });
  }
}
