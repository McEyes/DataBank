import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import * as i0 from "@angular/core";
export class ExcelExportService {
    constructor() {
        this.infoList = [];
        this.fileName = 'download';
    }
    exp(data, exportColumns, fileName) {
        this.fileName = fileName || 'download';
        let worksheet = {};
        if (data.length !== 0) {
            this.downloadData(data, worksheet, exportColumns);
        }
        else {
            console.log('no data');
        }
    }
    downloadData(scoredata, worksheet, exportColumns) {
        //set excel cell width
        var long = {
            '001': [
                { wch: 20 },
                { wch: 15 },
                { wch: 30 },
                { wch: 20 },
                { wch: 20 },
                { wch: 20 },
                { wch: 20 },
            ],
        };
        //excel set first row height
        var row = {
            '001': [{ hpt: 30 }],
        };
        // console.log('export', exportColumns);
        let result = [];
        if (scoredata.length) {
            //handle data
            if (exportColumns.length && (scoredata.length === exportColumns.length)) {
                scoredata.forEach((element, i) => {
                    let obj = {};
                    for (var key in element) {
                        obj[exportColumns[i]] = element[key];
                        // let has = false;
                        // for (var item of exportColumns) {
                        //   if (item.index == key) {
                        //     has = true;
                        //   }
                        // }
                    }
                    result.push(obj);
                });
                scoredata = result;
            }
            worksheet = XLSX.utils.json_to_sheet(scoredata);
            let header = exportColumns;
            //excel header en translate to zh
            // for (let i = Number('A'.charCodeAt(0)); i <= Number('V'.charCodeAt(0)); i++) {
            //   header.push(String.fromCharCode(i) + '1');
            // }
            //set excel cell height and width
            worksheet['!cols'] = long['001'];
            worksheet['!rows'] = row['001'];
            const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
            const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
            //save excel
            this.saveAsExcelFile(excelBuffer, this.fileName);
        }
        else {
            console.log('no data');
        }
    }
    saveAsExcelFile(buffer, fileName) {
        const data = new Blob([buffer], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
        });
        FileSaver.saveAs(data, fileName + '.xlsx');
    }
}
ExcelExportService.ɵfac = function ExcelExportService_Factory(t) { return new (t || ExcelExportService)(); };
ExcelExportService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: ExcelExportService, factory: ExcelExportService.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(ExcelExportService, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], null, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZXhjZWwtZXhwb3J0LnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2UvZXhjZWwtZXhwb3J0L2V4Y2VsLWV4cG9ydC5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxLQUFLLFNBQVMsTUFBTSxZQUFZLENBQUM7QUFDeEMsT0FBTyxLQUFLLElBQUksTUFBTSxNQUFNLENBQUM7O0FBSzdCLE1BQU0sT0FBTyxrQkFBa0I7SUFIL0I7UUFJRSxhQUFRLEdBQWUsRUFBRSxDQUFDO1FBQ2xCLGFBQVEsR0FBVyxVQUFVLENBQUM7S0E0RXZDO0lBMUVDLEdBQUcsQ0FBQyxJQUFnQixFQUFFLGFBQTBCLEVBQUUsUUFBaUI7UUFDakUsSUFBSSxDQUFDLFFBQVEsR0FBRyxRQUFRLElBQUksVUFBVSxDQUFDO1FBQ3ZDLElBQUksU0FBUyxHQUFtQixFQUFFLENBQUM7UUFDbkMsSUFBSSxJQUFJLENBQUMsTUFBTSxLQUFLLENBQUMsRUFBRTtZQUNyQixJQUFJLENBQUMsWUFBWSxDQUFDLElBQUksRUFBRSxTQUFTLEVBQUUsYUFBYSxDQUFDLENBQUM7U0FDbkQ7YUFBTTtZQUNMLE9BQU8sQ0FBQyxHQUFHLENBQUMsU0FBUyxDQUFDLENBQUM7U0FDeEI7SUFDSCxDQUFDO0lBRUQsWUFBWSxDQUFDLFNBQXFCLEVBQUUsU0FBYyxFQUFFLGFBQW1CO1FBQ3JFLHNCQUFzQjtRQUN0QixJQUFJLElBQUksR0FBRztZQUNULEtBQUssRUFBRTtnQkFDTCxFQUFFLEdBQUcsRUFBRSxFQUFFLEVBQUU7Z0JBQ1gsRUFBRSxHQUFHLEVBQUUsRUFBRSxFQUFFO2dCQUNYLEVBQUUsR0FBRyxFQUFFLEVBQUUsRUFBRTtnQkFDWCxFQUFFLEdBQUcsRUFBRSxFQUFFLEVBQUU7Z0JBQ1gsRUFBRSxHQUFHLEVBQUUsRUFBRSxFQUFFO2dCQUNYLEVBQUUsR0FBRyxFQUFFLEVBQUUsRUFBRTtnQkFDWCxFQUFFLEdBQUcsRUFBRSxFQUFFLEVBQUU7YUFDWjtTQUNGLENBQUM7UUFDRiw0QkFBNEI7UUFDNUIsSUFBSSxHQUFHLEdBQUc7WUFDUixLQUFLLEVBQUUsQ0FBQyxFQUFFLEdBQUcsRUFBRSxFQUFFLEVBQUUsQ0FBQztTQUNyQixDQUFDO1FBQ0Ysd0NBQXdDO1FBQ3hDLElBQUksTUFBTSxHQUFlLEVBQUUsQ0FBQTtRQUMzQixJQUFJLFNBQVMsQ0FBQyxNQUFNLEVBQUU7WUFDcEIsYUFBYTtZQUNiLElBQUcsYUFBYSxDQUFDLE1BQU0sSUFBSSxDQUFDLFNBQVMsQ0FBQyxNQUFNLEtBQUssYUFBYSxDQUFDLE1BQU0sQ0FBQyxFQUFDO2dCQUNyRSxTQUFTLENBQUMsT0FBTyxDQUFDLENBQUMsT0FBWSxFQUFFLENBQUMsRUFBRSxFQUFFO29CQUNwQyxJQUFJLEdBQUcsR0FBUSxFQUFFLENBQUE7b0JBQ2pCLEtBQUssSUFBSSxHQUFHLElBQUksT0FBTyxFQUFFO3dCQUN2QixHQUFHLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLEdBQUcsQ0FBQyxDQUFBO3dCQUNwQyxtQkFBbUI7d0JBQ25CLG9DQUFvQzt3QkFDcEMsNkJBQTZCO3dCQUM3QixrQkFBa0I7d0JBQ2xCLE1BQU07d0JBQ04sSUFBSTtxQkFDTDtvQkFFRCxNQUFNLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFBO2dCQUNsQixDQUFDLENBQUMsQ0FBQztnQkFFSCxTQUFTLEdBQUcsTUFBTSxDQUFBO2FBQ25CO1lBRUQsU0FBUyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsYUFBYSxDQUFDLFNBQVMsQ0FBQyxDQUFDO1lBQ2hELElBQUksTUFBTSxHQUFHLGFBQWEsQ0FBQztZQUMzQixpQ0FBaUM7WUFDakMsaUZBQWlGO1lBQ2pGLCtDQUErQztZQUMvQyxJQUFJO1lBQ0osaUNBQWlDO1lBQ2pDLFNBQVMsQ0FBQyxPQUFPLENBQUMsR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDakMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxHQUFHLEdBQUcsQ0FBQyxLQUFLLENBQUMsQ0FBQztZQUNoQyxNQUFNLFFBQVEsR0FBa0IsRUFBRSxNQUFNLEVBQUUsRUFBRSxJQUFJLEVBQUUsU0FBUyxFQUFFLEVBQUUsVUFBVSxFQUFFLENBQUMsTUFBTSxDQUFDLEVBQUUsQ0FBQztZQUN0RixNQUFNLFdBQVcsR0FBUSxJQUFJLENBQUMsS0FBSyxDQUFDLFFBQVEsRUFBRSxFQUFFLFFBQVEsRUFBRSxNQUFNLEVBQUUsSUFBSSxFQUFFLE9BQU8sRUFBRSxDQUFDLENBQUM7WUFDbkYsWUFBWTtZQUNaLElBQUksQ0FBQyxlQUFlLENBQUMsV0FBVyxFQUFFLElBQUksQ0FBQyxRQUFRLENBQUMsQ0FBQztTQUNsRDthQUFNO1lBQ0wsT0FBTyxDQUFDLEdBQUcsQ0FBQyxTQUFTLENBQUMsQ0FBQztTQUN4QjtJQUNILENBQUM7SUFFRCxlQUFlLENBQUMsTUFBVyxFQUFFLFFBQWdCO1FBQzNDLE1BQU0sSUFBSSxHQUFTLElBQUksSUFBSSxDQUFDLENBQUMsTUFBTSxDQUFDLEVBQUU7WUFDcEMsSUFBSSxFQUFFLGlGQUFpRjtTQUN4RixDQUFDLENBQUM7UUFDSCxTQUFTLENBQUMsTUFBTSxDQUFDLElBQUksRUFBRSxRQUFRLEdBQUcsT0FBTyxDQUFDLENBQUM7SUFDN0MsQ0FBQzs7b0ZBN0VVLGtCQUFrQjt3RUFBbEIsa0JBQWtCLFdBQWxCLGtCQUFrQixtQkFGakIsTUFBTTt1RkFFUCxrQkFBa0I7Y0FIOUIsVUFBVTtlQUFDO2dCQUNWLFVBQVUsRUFBRSxNQUFNO2FBQ25CIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgSW5qZWN0YWJsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgKiBhcyBGaWxlU2F2ZXIgZnJvbSAnZmlsZS1zYXZlcic7XHJcbmltcG9ydCAqIGFzIFhMU1ggZnJvbSAneGxzeCc7XHJcblxyXG5ASW5qZWN0YWJsZSh7XHJcbiAgcHJvdmlkZWRJbjogJ3Jvb3QnLFxyXG59KVxyXG5leHBvcnQgY2xhc3MgRXhjZWxFeHBvcnRTZXJ2aWNlIHtcclxuICBpbmZvTGlzdDogQXJyYXk8YW55PiA9IFtdO1xyXG4gIHByaXZhdGUgZmlsZU5hbWU6IHN0cmluZyA9ICdkb3dubG9hZCc7XHJcblxyXG4gIGV4cChkYXRhOiBBcnJheTxhbnk+LCBleHBvcnRDb2x1bW5zPzogQXJyYXk8YW55PiwgZmlsZU5hbWU/OiBzdHJpbmcpIHtcclxuICAgIHRoaXMuZmlsZU5hbWUgPSBmaWxlTmFtZSB8fCAnZG93bmxvYWQnO1xyXG4gICAgbGV0IHdvcmtzaGVldDogWExTWC5Xb3JrU2hlZXQgPSB7fTtcclxuICAgIGlmIChkYXRhLmxlbmd0aCAhPT0gMCkge1xyXG4gICAgICB0aGlzLmRvd25sb2FkRGF0YShkYXRhLCB3b3Jrc2hlZXQsIGV4cG9ydENvbHVtbnMpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgY29uc29sZS5sb2coJ25vIGRhdGEnKTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGRvd25sb2FkRGF0YShzY29yZWRhdGE6IEFycmF5PGFueT4sIHdvcmtzaGVldDogYW55LCBleHBvcnRDb2x1bW5zPzogYW55KSB7XHJcbiAgICAvL3NldCBleGNlbCBjZWxsIHdpZHRoXHJcbiAgICB2YXIgbG9uZyA9IHtcclxuICAgICAgJzAwMSc6IFtcclxuICAgICAgICB7IHdjaDogMjAgfSxcclxuICAgICAgICB7IHdjaDogMTUgfSxcclxuICAgICAgICB7IHdjaDogMzAgfSxcclxuICAgICAgICB7IHdjaDogMjAgfSxcclxuICAgICAgICB7IHdjaDogMjAgfSxcclxuICAgICAgICB7IHdjaDogMjAgfSxcclxuICAgICAgICB7IHdjaDogMjAgfSxcclxuICAgICAgXSxcclxuICAgIH07XHJcbiAgICAvL2V4Y2VsIHNldCBmaXJzdCByb3cgaGVpZ2h0XHJcbiAgICB2YXIgcm93ID0ge1xyXG4gICAgICAnMDAxJzogW3sgaHB0OiAzMCB9XSxcclxuICAgIH07XHJcbiAgICAvLyBjb25zb2xlLmxvZygnZXhwb3J0JywgZXhwb3J0Q29sdW1ucyk7XHJcbiAgICBsZXQgcmVzdWx0OiBBcnJheTxhbnk+ID0gW11cclxuICAgIGlmIChzY29yZWRhdGEubGVuZ3RoKSB7XHJcbiAgICAgIC8vaGFuZGxlIGRhdGFcclxuICAgICAgaWYoZXhwb3J0Q29sdW1ucy5sZW5ndGggJiYgKHNjb3JlZGF0YS5sZW5ndGggPT09IGV4cG9ydENvbHVtbnMubGVuZ3RoKSl7XHJcbiAgICAgICAgc2NvcmVkYXRhLmZvckVhY2goKGVsZW1lbnQ6IGFueSwgaSkgPT4ge1xyXG4gICAgICAgICAgbGV0IG9iajogYW55ID0ge31cclxuICAgICAgICAgIGZvciAodmFyIGtleSBpbiBlbGVtZW50KSB7XHJcbiAgICAgICAgICAgIG9ialtleHBvcnRDb2x1bW5zW2ldXSA9IGVsZW1lbnRba2V5XVxyXG4gICAgICAgICAgICAvLyBsZXQgaGFzID0gZmFsc2U7XHJcbiAgICAgICAgICAgIC8vIGZvciAodmFyIGl0ZW0gb2YgZXhwb3J0Q29sdW1ucykge1xyXG4gICAgICAgICAgICAvLyAgIGlmIChpdGVtLmluZGV4ID09IGtleSkge1xyXG4gICAgICAgICAgICAvLyAgICAgaGFzID0gdHJ1ZTtcclxuICAgICAgICAgICAgLy8gICB9XHJcbiAgICAgICAgICAgIC8vIH1cclxuICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICByZXN1bHQucHVzaChvYmopXHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIHNjb3JlZGF0YSA9IHJlc3VsdFxyXG4gICAgICB9XHJcblxyXG4gICAgICB3b3Jrc2hlZXQgPSBYTFNYLnV0aWxzLmpzb25fdG9fc2hlZXQoc2NvcmVkYXRhKTtcclxuICAgICAgbGV0IGhlYWRlciA9IGV4cG9ydENvbHVtbnM7XHJcbiAgICAgIC8vZXhjZWwgaGVhZGVyIGVuIHRyYW5zbGF0ZSB0byB6aFxyXG4gICAgICAvLyBmb3IgKGxldCBpID0gTnVtYmVyKCdBJy5jaGFyQ29kZUF0KDApKTsgaSA8PSBOdW1iZXIoJ1YnLmNoYXJDb2RlQXQoMCkpOyBpKyspIHtcclxuICAgICAgLy8gICBoZWFkZXIucHVzaChTdHJpbmcuZnJvbUNoYXJDb2RlKGkpICsgJzEnKTtcclxuICAgICAgLy8gfVxyXG4gICAgICAvL3NldCBleGNlbCBjZWxsIGhlaWdodCBhbmQgd2lkdGhcclxuICAgICAgd29ya3NoZWV0WychY29scyddID0gbG9uZ1snMDAxJ107XHJcbiAgICAgIHdvcmtzaGVldFsnIXJvd3MnXSA9IHJvd1snMDAxJ107XHJcbiAgICAgIGNvbnN0IHdvcmtib29rOiBYTFNYLldvcmtCb29rID0geyBTaGVldHM6IHsgZGF0YTogd29ya3NoZWV0IH0sIFNoZWV0TmFtZXM6IFsnZGF0YSddIH07XHJcbiAgICAgIGNvbnN0IGV4Y2VsQnVmZmVyOiBhbnkgPSBYTFNYLndyaXRlKHdvcmtib29rLCB7IGJvb2tUeXBlOiAneGxzeCcsIHR5cGU6ICdhcnJheScgfSk7XHJcbiAgICAgIC8vc2F2ZSBleGNlbFxyXG4gICAgICB0aGlzLnNhdmVBc0V4Y2VsRmlsZShleGNlbEJ1ZmZlciwgdGhpcy5maWxlTmFtZSk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICBjb25zb2xlLmxvZygnbm8gZGF0YScpO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgc2F2ZUFzRXhjZWxGaWxlKGJ1ZmZlcjogYW55LCBmaWxlTmFtZTogc3RyaW5nKSB7XHJcbiAgICBjb25zdCBkYXRhOiBCbG9iID0gbmV3IEJsb2IoW2J1ZmZlcl0sIHtcclxuICAgICAgdHlwZTogJ2FwcGxpY2F0aW9uL3ZuZC5vcGVueG1sZm9ybWF0cy1vZmZpY2Vkb2N1bWVudC5zcHJlYWRzaGVldG1sLnNoZWV0O2NoYXJzZXQ9VVRGLTgnLFxyXG4gICAgfSk7XHJcbiAgICBGaWxlU2F2ZXIuc2F2ZUFzKGRhdGEsIGZpbGVOYW1lICsgJy54bHN4Jyk7XHJcbiAgfVxyXG59XHJcbiJdfQ==