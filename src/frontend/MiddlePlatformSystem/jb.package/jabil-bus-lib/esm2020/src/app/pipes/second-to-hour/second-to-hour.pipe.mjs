import { Pipe } from '@angular/core';
import * as i0 from "@angular/core";
// import distanceInWordsToNow from 'date-fns/distance_in_words_to_now';
// import dateFns from 'date-fns';
export class SecondToHour {
    /**
     * Takes a value and makes it lowercase.
     */
    transform(value, ...args) {
        if (!value) {
            return '';
        }
        //  秒
        let second = value;
        //  分
        let minute = 0;
        //  小时
        let hour = 0;
        //  天
        //  let day = 0
        //  如果秒数大于60，将秒数转换成整数
        if (second > 60) {
            //  获取分钟，除以60取整数，得到整数分钟
            minute = parseInt((second / 60).toString());
            //  获取秒数，秒数取佘，得到整数秒数
            second = parseInt((second % 60).toString());
            //  如果分钟大于60，将分钟转换成小时
            if (minute > 60) {
                //  获取小时，获取分钟除以60，得到整数小时
                hour = parseInt((minute / 60).toString());
                //  获取小时后取佘的分，获取分钟除以60取佘的分
                minute = parseInt((minute % 60).toString());
                //  如果小时大于24，将小时转换成天
                //  if (hour > 23) {
                //    //  获取天数，获取小时除以24，得到整天数
                //    day = parseInt(hour / 24)
                //    //  获取天数后取余的小时，获取小时除以24取余的小时
                //    hour = parseInt(hour % 24)
                //  }
            }
        }
        let result = '' + parseInt(second.toString()) + 'S';
        if (minute > 0) {
            result = '' + parseInt(minute.toString()) + 'M' + result;
        }
        if (hour > 0) {
            result = '' + parseInt(hour.toString()) + 'H' + result;
        }
        //  if (day > 0) {
        //    result = '' + parseInt(day) + '天' + result
        //  }
        return result;
    }
}
SecondToHour.ɵfac = function SecondToHour_Factory(t) { return new (t || SecondToHour)(); };
SecondToHour.ɵpipe = /*@__PURE__*/ i0.ɵɵdefinePipe({ name: "SecondToHour", type: SecondToHour, pure: true });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(SecondToHour, [{
        type: Pipe,
        args: [{
                name: 'SecondToHour',
            }]
    }], null, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoic2Vjb25kLXRvLWhvdXIucGlwZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvcGlwZXMvc2Vjb25kLXRvLWhvdXIvc2Vjb25kLXRvLWhvdXIucGlwZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsSUFBSSxFQUFpQixNQUFNLGVBQWUsQ0FBQzs7QUFDcEQsd0VBQXdFO0FBQ3hFLGtDQUFrQztBQUtsQyxNQUFNLE9BQU8sWUFBWTtJQUN2Qjs7T0FFRztJQUNILFNBQVMsQ0FBQyxLQUFhLEVBQUUsR0FBRyxJQUFnQjtRQUMxQyxJQUFJLENBQUMsS0FBSyxFQUFFO1lBQ1YsT0FBTyxFQUFFLENBQUM7U0FDWDtRQUVELEtBQUs7UUFDTCxJQUFJLE1BQU0sR0FBVyxLQUFLLENBQUM7UUFDM0IsS0FBSztRQUNMLElBQUksTUFBTSxHQUFHLENBQUMsQ0FBQztRQUNmLE1BQU07UUFDTixJQUFJLElBQUksR0FBRyxDQUFDLENBQUM7UUFDYixLQUFLO1FBQ0wsZUFBZTtRQUNmLHFCQUFxQjtRQUNyQixJQUFJLE1BQU0sR0FBRyxFQUFFLEVBQUU7WUFDZix1QkFBdUI7WUFDdkIsTUFBTSxHQUFHLFFBQVEsQ0FBQyxDQUFDLE1BQU0sR0FBRyxFQUFFLENBQUMsQ0FBQyxRQUFRLEVBQUUsQ0FBQyxDQUFDO1lBQzVDLG9CQUFvQjtZQUNwQixNQUFNLEdBQUcsUUFBUSxDQUFDLENBQUMsTUFBTSxHQUFHLEVBQUUsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUM7WUFDNUMscUJBQXFCO1lBQ3JCLElBQUksTUFBTSxHQUFHLEVBQUUsRUFBRTtnQkFDZix3QkFBd0I7Z0JBQ3hCLElBQUksR0FBRyxRQUFRLENBQUMsQ0FBQyxNQUFNLEdBQUcsRUFBRSxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUMsQ0FBQztnQkFDMUMsMEJBQTBCO2dCQUMxQixNQUFNLEdBQUcsUUFBUSxDQUFDLENBQUMsTUFBTSxHQUFHLEVBQUUsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUM7Z0JBQzVDLG9CQUFvQjtnQkFDcEIsb0JBQW9CO2dCQUNwQiw2QkFBNkI7Z0JBQzdCLCtCQUErQjtnQkFDL0Isa0NBQWtDO2dCQUNsQyxnQ0FBZ0M7Z0JBQ2hDLEtBQUs7YUFDTjtTQUNGO1FBRUQsSUFBSSxNQUFNLEdBQUcsRUFBRSxHQUFHLFFBQVEsQ0FBQyxNQUFNLENBQUMsUUFBUSxFQUFFLENBQUMsR0FBRyxHQUFHLENBQUM7UUFDcEQsSUFBSSxNQUFNLEdBQUcsQ0FBQyxFQUFFO1lBQ2QsTUFBTSxHQUFHLEVBQUUsR0FBRyxRQUFRLENBQUMsTUFBTSxDQUFDLFFBQVEsRUFBRSxDQUFDLEdBQUcsR0FBRyxHQUFHLE1BQU0sQ0FBQztTQUMxRDtRQUNELElBQUksSUFBSSxHQUFHLENBQUMsRUFBRTtZQUNaLE1BQU0sR0FBRyxFQUFFLEdBQUcsUUFBUSxDQUFDLElBQUksQ0FBQyxRQUFRLEVBQUUsQ0FBQyxHQUFHLEdBQUcsR0FBRyxNQUFNLENBQUM7U0FDeEQ7UUFDRCxrQkFBa0I7UUFDbEIsZ0RBQWdEO1FBQ2hELEtBQUs7UUFDTCxPQUFPLE1BQU0sQ0FBQztJQUNoQixDQUFDOzt3RUFsRFUsWUFBWTtpRkFBWixZQUFZO3VGQUFaLFlBQVk7Y0FIeEIsSUFBSTtlQUFDO2dCQUNKLElBQUksRUFBRSxjQUFjO2FBQ3JCIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgUGlwZSwgUGlwZVRyYW5zZm9ybSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG4vLyBpbXBvcnQgZGlzdGFuY2VJbldvcmRzVG9Ob3cgZnJvbSAnZGF0ZS1mbnMvZGlzdGFuY2VfaW5fd29yZHNfdG9fbm93JztcclxuLy8gaW1wb3J0IGRhdGVGbnMgZnJvbSAnZGF0ZS1mbnMnO1xyXG5cclxuQFBpcGUoe1xyXG4gIG5hbWU6ICdTZWNvbmRUb0hvdXInLFxyXG59KVxyXG5leHBvcnQgY2xhc3MgU2Vjb25kVG9Ib3VyIGltcGxlbWVudHMgUGlwZVRyYW5zZm9ybSB7XHJcbiAgLyoqXHJcbiAgICogVGFrZXMgYSB2YWx1ZSBhbmQgbWFrZXMgaXQgbG93ZXJjYXNlLlxyXG4gICAqL1xyXG4gIHRyYW5zZm9ybSh2YWx1ZTogbnVtYmVyLCAuLi5hcmdzOiBBcnJheTxhbnk+KSB7XHJcbiAgICBpZiAoIXZhbHVlKSB7XHJcbiAgICAgIHJldHVybiAnJztcclxuICAgIH1cclxuXHJcbiAgICAvLyAg56eSXHJcbiAgICBsZXQgc2Vjb25kOiBudW1iZXIgPSB2YWx1ZTtcclxuICAgIC8vICDliIZcclxuICAgIGxldCBtaW51dGUgPSAwO1xyXG4gICAgLy8gIOWwj+aXtlxyXG4gICAgbGV0IGhvdXIgPSAwO1xyXG4gICAgLy8gIOWkqVxyXG4gICAgLy8gIGxldCBkYXkgPSAwXHJcbiAgICAvLyAg5aaC5p6c56eS5pWw5aSn5LqONjDvvIzlsIbnp5LmlbDovazmjaLmiJDmlbTmlbBcclxuICAgIGlmIChzZWNvbmQgPiA2MCkge1xyXG4gICAgICAvLyAg6I635Y+W5YiG6ZKf77yM6Zmk5LulNjDlj5bmlbTmlbDvvIzlvpfliLDmlbTmlbDliIbpkp9cclxuICAgICAgbWludXRlID0gcGFyc2VJbnQoKHNlY29uZCAvIDYwKS50b1N0cmluZygpKTtcclxuICAgICAgLy8gIOiOt+WPluenkuaVsO+8jOenkuaVsOWPluS9mO+8jOW+l+WIsOaVtOaVsOenkuaVsFxyXG4gICAgICBzZWNvbmQgPSBwYXJzZUludCgoc2Vjb25kICUgNjApLnRvU3RyaW5nKCkpO1xyXG4gICAgICAvLyAg5aaC5p6c5YiG6ZKf5aSn5LqONjDvvIzlsIbliIbpkp/ovazmjaLmiJDlsI/ml7ZcclxuICAgICAgaWYgKG1pbnV0ZSA+IDYwKSB7XHJcbiAgICAgICAgLy8gIOiOt+WPluWwj+aXtu+8jOiOt+WPluWIhumSn+mZpOS7pTYw77yM5b6X5Yiw5pW05pWw5bCP5pe2XHJcbiAgICAgICAgaG91ciA9IHBhcnNlSW50KChtaW51dGUgLyA2MCkudG9TdHJpbmcoKSk7XHJcbiAgICAgICAgLy8gIOiOt+WPluWwj+aXtuWQjuWPluS9mOeahOWIhu+8jOiOt+WPluWIhumSn+mZpOS7pTYw5Y+W5L2Y55qE5YiGXHJcbiAgICAgICAgbWludXRlID0gcGFyc2VJbnQoKG1pbnV0ZSAlIDYwKS50b1N0cmluZygpKTtcclxuICAgICAgICAvLyAg5aaC5p6c5bCP5pe25aSn5LqOMjTvvIzlsIblsI/ml7bovazmjaLmiJDlpKlcclxuICAgICAgICAvLyAgaWYgKGhvdXIgPiAyMykge1xyXG4gICAgICAgIC8vICAgIC8vICDojrflj5blpKnmlbDvvIzojrflj5blsI/ml7bpmaTku6UyNO+8jOW+l+WIsOaVtOWkqeaVsFxyXG4gICAgICAgIC8vICAgIGRheSA9IHBhcnNlSW50KGhvdXIgLyAyNClcclxuICAgICAgICAvLyAgICAvLyAg6I635Y+W5aSp5pWw5ZCO5Y+W5L2Z55qE5bCP5pe277yM6I635Y+W5bCP5pe26Zmk5LulMjTlj5bkvZnnmoTlsI/ml7ZcclxuICAgICAgICAvLyAgICBob3VyID0gcGFyc2VJbnQoaG91ciAlIDI0KVxyXG4gICAgICAgIC8vICB9XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBsZXQgcmVzdWx0ID0gJycgKyBwYXJzZUludChzZWNvbmQudG9TdHJpbmcoKSkgKyAnUyc7XHJcbiAgICBpZiAobWludXRlID4gMCkge1xyXG4gICAgICByZXN1bHQgPSAnJyArIHBhcnNlSW50KG1pbnV0ZS50b1N0cmluZygpKSArICdNJyArIHJlc3VsdDtcclxuICAgIH1cclxuICAgIGlmIChob3VyID4gMCkge1xyXG4gICAgICByZXN1bHQgPSAnJyArIHBhcnNlSW50KGhvdXIudG9TdHJpbmcoKSkgKyAnSCcgKyByZXN1bHQ7XHJcbiAgICB9XHJcbiAgICAvLyAgaWYgKGRheSA+IDApIHtcclxuICAgIC8vICAgIHJlc3VsdCA9ICcnICsgcGFyc2VJbnQoZGF5KSArICflpKknICsgcmVzdWx0XHJcbiAgICAvLyAgfVxyXG4gICAgcmV0dXJuIHJlc3VsdDtcclxuICB9XHJcbn1cclxuIl19