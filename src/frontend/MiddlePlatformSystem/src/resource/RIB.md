# 1 jQuery 库包含以下功能：
- HTML 元素选取
- HTML 元素操作
- CSS 操作
- HTML 事件函数
- JavaScript 特效和动画
- HTML DOM 遍历和修改
- AJAX
- Utilities

# 2 选择器
- $("tr:even")	选取偶数位置的 <tr> 元素（odd奇数）
- $(":button")	选取所有 type="button" 的 input 元素 和 button 元素
- $("a[target='_blank']")	选取所有 target 属性值等于 "_blank" 的 <a> 元素

# 3 事件
- click	
- keypress	
- submit	
- load
- dblclick	
- keydown	
- change	
- resize
- mouseenter	
- keyup	
- focus
- scroll
- mouseleave	 
- blur
- unload
- hover

# 4 效果
- hide
- show
- toggle
- fadeIn
- fadeOut
- fadeToggle
- fadeTo 透明度
- slideDown 
- slideUp 
- slideToggle 
- $(selector).animate({params},speed,callback); Camel 标记法书写所有的属性名
- stop

# 5 HTML
- text() - 设置或返回所选元素的文本内容
- html() - 设置或返回所选元素的内容（包括 HTML 标签）
- val() - 设置或返回表单字段的值
- attr() 获取属性 设置属性 attr("href","http://www.runoob.com/jquery");
- append() 在被选元素的结尾插入内容，仍在元素里
- prepend() 在被选元素的开头插入内容，仍在元素里
- after() 在被选元素之后插入内容
- before() 在被选元素之前插入内容
- remove() - 删除被选元素（及其子元素）
- empty() - 从被选元素中删除子元素
- addClass() - 向被选元素添加一个或多个类
- removeClass() - 从被选元素删除一个或多个类
- toggleClass() - 对被选元素进行添加/删除类的切换操作
- css() 
- width() 元素
- height() 元素
- innerWidth() 元素+padding
- innerHeight() 元素+padding
- outerWidth() 元素+padding + border
- outerHeight()  元素+padding + border
- outerWidth(true) 元素+padding + border + margin
- outerHeight(true)  元素+padding + border + margin

# 6 遍历
- parent()
- parents() 所有祖先
- parentsUntil() 方法返回介于两个给定元素之间的所有祖先元素 $("span").parentsUntil("div");
- children()
- find()
- siblings() 所有同胞元素
- next() 
- nextAll()
- nextUntil()
- prev()
- prevAll()
- prevUntil()
- first() 内部第一个元素
- last()
- eq()索引
- filter()
- not()

# 7 Ajax
- load() $(selector).load(URL,data,callback);  $("#div1").load("demo_test.txt"); $("#div1").load("demo_test.txt #p1");
- get() 
$.get(URL,callback);
或
$.get( URL [, data ] [, callback ] [, dataType ] )
- post() $.post(URL,callback);
或
$.post( URL [, data ] [, callback ] [, dataType ] )

noConflict() 方法 处理$符号冲突
$.noConflict();
jQuery(document).ready(function(){
  jQuery("button").click(function(){
    jQuery("p").text("jQuery 仍然在工作!");
  });
});

https://www.runoob.com/jquery/ajax-ajax.html