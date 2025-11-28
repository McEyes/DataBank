import { Pipe } from '@angular/core';
import * as i0 from "@angular/core";
// import distanceInWordsToNow from 'date-fns/distance_in_words_to_now';
// import dateFns from 'date-fns';
export class FormatTime {
    /**
     * transform datetime.
     */
    transform(value, ...args) {
        if (!value) {
            return '';
        }
        let result = new Date(value);
        // 24: getDay(): 获取传入的日期是星期几
        // 25: getMonth(): 返回传入时间的月份
        // 26: getMinutes(): 返回传入时间的分钟数
        // 27:getHours():返回传入时间的小时数
        const month = result.getMonth() + 1 < 10 ? '0' + (result.getMonth() + 1) : result.getMonth() + 1;
        const day = result.getDate() < 10 ? '0' + result.getDate() : result.getDate();
        const hour = result.getHours() < 10 ? '0' + result.getHours() : result.getHours();
        const minute = result.getMinutes() < 10 ? '0' + result.getMinutes() : result.getMinutes();
        return result.getFullYear() + '-' + month + '-' + day + ' ' + hour + ':' + minute;
        // return distanceInWordsToNow(new Date(value), { addSuffix: true });
    }
}
FormatTime.ɵfac = function FormatTime_Factory(t) { return new (t || FormatTime)(); };
FormatTime.ɵpipe = /*@__PURE__*/ i0.ɵɵdefinePipe({ name: "FormatTime", type: FormatTime, pure: true });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FormatTime, [{
        type: Pipe,
        args: [{
                name: 'FormatTime',
            }]
    }], null, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZm9ybWF0LXRpbWUucGlwZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvcGlwZXMvZm9ybWF0LXRpbWUvZm9ybWF0LXRpbWUucGlwZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsSUFBSSxFQUFpQixNQUFNLGVBQWUsQ0FBQzs7QUFDcEQsd0VBQXdFO0FBQ3hFLGtDQUFrQztBQUtsQyxNQUFNLE9BQU8sVUFBVTtJQUNyQjs7T0FFRztJQUNILFNBQVMsQ0FBQyxLQUFhLEVBQUUsR0FBRyxJQUFnQjtRQUMxQyxJQUFJLENBQUMsS0FBSyxFQUFFO1lBQ1YsT0FBTyxFQUFFLENBQUM7U0FDWDtRQUNELElBQUksTUFBTSxHQUFHLElBQUksSUFBSSxDQUFDLEtBQUssQ0FBQyxDQUFDO1FBQzdCLDRCQUE0QjtRQUM1Qiw0QkFBNEI7UUFDNUIsK0JBQStCO1FBQy9CLDJCQUEyQjtRQUMzQixNQUFNLEtBQUssR0FDVCxNQUFNLENBQUMsUUFBUSxFQUFFLEdBQUcsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDLENBQUMsR0FBRyxHQUFHLENBQUMsTUFBTSxDQUFDLFFBQVEsRUFBRSxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxNQUFNLENBQUMsUUFBUSxFQUFFLEdBQUcsQ0FBQyxDQUFDO1FBQ3JGLE1BQU0sR0FBRyxHQUFHLE1BQU0sQ0FBQyxPQUFPLEVBQUUsR0FBRyxFQUFFLENBQUMsQ0FBQyxDQUFDLEdBQUcsR0FBRyxNQUFNLENBQUMsT0FBTyxFQUFFLENBQUMsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxPQUFPLEVBQUUsQ0FBQztRQUM5RSxNQUFNLElBQUksR0FBRyxNQUFNLENBQUMsUUFBUSxFQUFFLEdBQUcsRUFBRSxDQUFDLENBQUMsQ0FBQyxHQUFHLEdBQUcsTUFBTSxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUMsQ0FBQyxNQUFNLENBQUMsUUFBUSxFQUFFLENBQUM7UUFDbEYsTUFBTSxNQUFNLEdBQUcsTUFBTSxDQUFDLFVBQVUsRUFBRSxHQUFHLEVBQUUsQ0FBQyxDQUFDLENBQUMsR0FBRyxHQUFHLE1BQU0sQ0FBQyxVQUFVLEVBQUUsQ0FBQyxDQUFDLENBQUMsTUFBTSxDQUFDLFVBQVUsRUFBRSxDQUFDO1FBRTFGLE9BQU8sTUFBTSxDQUFDLFdBQVcsRUFBRSxHQUFHLEdBQUcsR0FBRyxLQUFLLEdBQUcsR0FBRyxHQUFHLEdBQUcsR0FBRyxHQUFHLEdBQUcsSUFBSSxHQUFHLEdBQUcsR0FBRyxNQUFNLENBQUM7UUFDbEYscUVBQXFFO0lBQ3ZFLENBQUM7O29FQXJCVSxVQUFVOzZFQUFWLFVBQVU7dUZBQVYsVUFBVTtjQUh0QixJQUFJO2VBQUM7Z0JBQ0osSUFBSSxFQUFFLFlBQVk7YUFDbkIiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBQaXBlLCBQaXBlVHJhbnNmb3JtIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbi8vIGltcG9ydCBkaXN0YW5jZUluV29yZHNUb05vdyBmcm9tICdkYXRlLWZucy9kaXN0YW5jZV9pbl93b3Jkc190b19ub3cnO1xyXG4vLyBpbXBvcnQgZGF0ZUZucyBmcm9tICdkYXRlLWZucyc7XHJcblxyXG5AUGlwZSh7XHJcbiAgbmFtZTogJ0Zvcm1hdFRpbWUnLFxyXG59KVxyXG5leHBvcnQgY2xhc3MgRm9ybWF0VGltZSBpbXBsZW1lbnRzIFBpcGVUcmFuc2Zvcm0ge1xyXG4gIC8qKlxyXG4gICAqIHRyYW5zZm9ybSBkYXRldGltZS5cclxuICAgKi9cclxuICB0cmFuc2Zvcm0odmFsdWU6IHN0cmluZywgLi4uYXJnczogQXJyYXk8YW55Pikge1xyXG4gICAgaWYgKCF2YWx1ZSkge1xyXG4gICAgICByZXR1cm4gJyc7XHJcbiAgICB9XHJcbiAgICBsZXQgcmVzdWx0ID0gbmV3IERhdGUodmFsdWUpO1xyXG4gICAgLy8gMjQ6IGdldERheSgpOiDojrflj5bkvKDlhaXnmoTml6XmnJ/mmK/mmJ/mnJ/lh6BcclxuICAgIC8vIDI1OiBnZXRNb250aCgpOiDov5Tlm57kvKDlhaXml7bpl7TnmoTmnIjku71cclxuICAgIC8vIDI2OiBnZXRNaW51dGVzKCk6IOi/lOWbnuS8oOWFpeaXtumXtOeahOWIhumSn+aVsFxyXG4gICAgLy8gMjc6Z2V0SG91cnMoKTrov5Tlm57kvKDlhaXml7bpl7TnmoTlsI/ml7bmlbBcclxuICAgIGNvbnN0IG1vbnRoID1cclxuICAgICAgcmVzdWx0LmdldE1vbnRoKCkgKyAxIDwgMTAgPyAnMCcgKyAocmVzdWx0LmdldE1vbnRoKCkgKyAxKSA6IHJlc3VsdC5nZXRNb250aCgpICsgMTtcclxuICAgIGNvbnN0IGRheSA9IHJlc3VsdC5nZXREYXRlKCkgPCAxMCA/ICcwJyArIHJlc3VsdC5nZXREYXRlKCkgOiByZXN1bHQuZ2V0RGF0ZSgpO1xyXG4gICAgY29uc3QgaG91ciA9IHJlc3VsdC5nZXRIb3VycygpIDwgMTAgPyAnMCcgKyByZXN1bHQuZ2V0SG91cnMoKSA6IHJlc3VsdC5nZXRIb3VycygpO1xyXG4gICAgY29uc3QgbWludXRlID0gcmVzdWx0LmdldE1pbnV0ZXMoKSA8IDEwID8gJzAnICsgcmVzdWx0LmdldE1pbnV0ZXMoKSA6IHJlc3VsdC5nZXRNaW51dGVzKCk7XHJcblxyXG4gICAgcmV0dXJuIHJlc3VsdC5nZXRGdWxsWWVhcigpICsgJy0nICsgbW9udGggKyAnLScgKyBkYXkgKyAnICcgKyBob3VyICsgJzonICsgbWludXRlO1xyXG4gICAgLy8gcmV0dXJuIGRpc3RhbmNlSW5Xb3Jkc1RvTm93KG5ldyBEYXRlKHZhbHVlKSwgeyBhZGRTdWZmaXg6IHRydWUgfSk7XHJcbiAgfVxyXG59XHJcbiJdfQ==