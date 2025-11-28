## 1.BFC
BFC实际就是页面一个独立的容器
BFC（Block Formatting Context），即块级格式化上下文，它是页面中的一块渲染区域，并且有一套属于自己的渲染规则：

内部的盒子会在垂直方向上一个接一个的放置
对于同一个BFC的俩个相邻的盒子的margin会发生重叠，与方向无关。
每个元素的左外边距与包含块的左边界相接触（从左到右），即使浮动元素也是如此
BFC的区域不会与float的元素区域重叠
计算BFC的高度时，浮动子元素也参与计算
BFC就是页面上的一个隔离的独立容器，容器里面的子元素不会影响到外面的元素，反之亦然
BFC目的是形成一个相对于外界完全独立的空间，让内部的子元素不会影响到外部的元素


触发BFC的条件包含不限于：

根元素，即HTML元素
浮动元素：float值为left、right
overflow值不为 visible，为 auto、scroll、hidden
display的值为inline-block、inltable-cell、table-caption、table、inline-table、flex、inline-flex、grid、inline-grid
position的值为absolute或fixed

## 2.居中
根据元素标签的性质，可以分为：

内联元素居中布局
块级元素居中布局
#内联元素居中布局
水平居中

行内元素可设置：text-align: center
flex布局设置父元素：display: flex; justify-content: center
垂直居中

单行文本父元素确认高度：height === line-height
多行文本父元素确认高度：display: table-cell; vertical-align: middle
#块级元素居中布局
水平居中

定宽: margin: 0 auto
绝对定位+left:50%+margin:负自身一半
垂直居中

position: absolute设置left、top、margin-left、margin-top(定高)
display: table-cell
transform: translate(x, y)
flex(不定高，不定宽)
grid(不定高，不定宽)，兼容性相对比较差

## css优化
实现方式有很多种，主要有如下：

内联首屏关键CSS
异步加载CSS
资源压缩
合理使用选择器
（不要嵌套使用过多复杂选择器，最好不要三层以上
使用id选择器就没必要再进行嵌套
通配符和属性选择器效率最低，避免使用）

减少使用昂贵的属性
在页面发生重绘的时候，昂贵属性如box-shadow/border-radius/filter/透明度/:nth-child等，会降低浏览器的渲染性能
不要使用@import

减少重排操作，以及减少不必要的重绘
了解哪些属性可以继承而来，避免对这些属性重复编写
cssSprite，合成所有icon图片，用宽高加上backgroud-position的背景图方式显现出我们要的icon图，减少了http请求
把小的icon图片转成base64编码
CSS3动画或者过渡尽量使用transform和opacity来实现动画，不要使用left和top属性