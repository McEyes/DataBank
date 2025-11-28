## 1.类型
基本数据类型存储在栈中

引用类型的对象存储于堆中

### 基本类型
- Number
- String
- Boolean
- Undefined
- null
- symbol
### 引用类型
- Object
- Array
- Function

## 2.数组
- push() 长度
- unshift() 长度
- splice() 返回空数组
- concat() 新数组
- pop() 元素
- shift() 元素
- splice() 返回空数组
- slice() 返回新数组 不会影响原始数组
- indexOf() let numbers = [1, 2, 3, 4, 5, 4, 3, 2, 1];
numbers.indexOf(4) // 3
- includes()
- find()
- reverse()
- sort()
- join()
- some()
- every()
- forEach()
- filter()
- map()

## 3字符串
- concat()
- slice()
- substr()
- substring()
let stringValue = "hello world";

console.log(stringValue.slice(3)); // "lo world"
console.log(stringValue.substring(3)); // "lo world"
console.log(stringValue.substr(3)); // "lo world"
console.log(stringValue.slice(3, 7)); // "lo w"
console.log(stringValue.substring(3,7)); // "lo w"
console.log(stringValue.substr(3, 7)); // "lo worl"

- trim()、trimLeft()、trimRight()

- repeat()
let stringValue = "na ";
let copyResult = stringValue.repeat(2) // na na 

- padStart()、padEnd()
let stringValue = "foo";
console.log(stringValue.padStart(6)); // " foo"
console.log(stringValue.padStart(9, ".")); // "......foo"

- toLowerCase()、 toUpperCase()

- chatAt()

- indexOf()

- startWith()

- includes()

- split()

- match()
- search()
- replace()

## 4.类型转换
显
Number()
parseInt()
String()
Boolean()
隐
我们这里可以归纳为两种情况发生隐式转换的场景：

比较运算（==、!=、>、<）、if、while需要布尔值地方
算术运算（+、-、*、/、%）

## 5.拷贝

### 在JavaScript中，存在浅拷贝的现象有：
function shallowClone(obj) {
    const newObj = {};
    for(let prop in obj) {
        if(obj.hasOwnProperty(prop)){
            newObj[prop] = obj[prop];
        }
    }
    return newObj;
}
Object.assign
Array.prototype.slice(), Array.prototype.concat()

### 6.常见的深拷贝方式有：

_.cloneDeep()

jQuery.extend()

JSON.stringify() 但是这种方式存在弊端，会忽略undefined、symbol和函数

手写循环递归
function deepClone(obj, hash = new WeakMap()) {
  if (obj === null) return obj; // 如果是null或者undefined我就不进行拷贝操作
  if (obj instanceof Date) return new Date(obj);
  if (obj instanceof RegExp) return new RegExp(obj);
  // 可能是对象或者普通的值  如果是函数的话是不需要深拷贝
  if (typeof obj !== "object") return obj;
  // 是对象的话就要进行深拷贝
  if (hash.get(obj)) return hash.get(obj);
  let cloneObj = new obj.constructor();
  // 找到的是所属类原型上的constructor,而原型上的 constructor指向的是当前类本身
  hash.set(obj, cloneObj);
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      // 实现一个递归拷贝
      cloneObj[key] = deepClone(obj[key], hash);
    }
  }
  return cloneObj;
}

## 7.闭包
有权访问里一个函数作用域变量的
任何闭包的使用场景都离不开这两点：

创建私有变量
延长变量的生命周期

柯里化函数 柯里化的目的在于避免频繁调用具有相同参数函数的同时，又能够轻松的重用

var Counter = (function() {
  var privateCounter = 0;
  function changeBy(val) {
    privateCounter += val;
  }
  return {
    increment: function() {
      changeBy(1);
    },
    decrement: function() {
      changeBy(-1);
    },
    value: function() {
      return privateCounter;
    }
  }
})();

## 8.作用域
作用域，即变量（变量作用域又称上下文）和函数生效（能被访问）的区域或集合

换句话说，作用域决定了代码区块中变量和其他资源的可见性
全局作用域

函数作用域

块级作用域

## 9.原型
每个对象拥有一个原型对象
当试图访问一个对象的属性时，它不仅仅在该对象上搜寻，还会搜寻该对象的原型，以及该对象的原型的原型，依次层层向上搜索，直到找到一个名字匹配的属性或到达原型链的末尾

### 原型对象也可能拥有原型，并从中继承方法和属性，一层一层、以此类推。这种关系常被称为原型链
每个对象的__proto__都是指向它的构造函数的原型对象prototype的

## 10.继承
// 货车
class Truck extends Car{
    constructor(color,speed){
        super(color,speed)
        this.Container = true // 货箱
    }
}

- 原型链继承

- 构造函数继承（借助 call）

- 组合继承

- 原型式继承
因为Object.create方法实现的是浅拷贝，多个实例的引用类型属性指向相同的内存，存在篡改的可能

- 寄生式继承
寄生式继承在上面继承基础上进行优化，利用这个浅拷贝的能力再进行增强，添加一些方法

- 寄生组合式继承

## 11.this
在绝大多数情况下，函数的调用方式决定了 this 的值（运行时绑定）

this 关键字是函数运行时自动生成的一个内部对象，只能在函数内部使用，总指向调用它的对象

this在函数执行过程中，this一旦被确定了，就不可以再更改

### 根据不同的使用场合，this有不同的值，主要分为下面几种情况：

- 默认绑定

- 隐式绑定

- new绑定
function test() {
　this.x = 1;
}

var obj = new test();
obj.x // 1

- 显示绑定
apply()、call()、bind()是函数的一个方法

综上，new绑定优先级 > 显示绑定优先级 > 隐式绑定优先级 > 默认绑定优先级

## 12.执行上下文
执行上下文的类型分为三种：

全局执行上下文：只有一个，浏览器中的全局对象就是 window对象，this 指向这个全局对象
函数执行上下文：存在无数个，只有在函数被调用的时候才会被创建，每次调用函数都会创建一个新的执行上下文
Eval 函数执行上下文： 指的是运行在 eval 函数中的代码，很少用而且不建议使用

## 13.事件流都会经历三个阶段：

- 事件捕获阶段(capture phase)
- 处于目标阶段(target phase)
- 事件冒泡阶段(bubbling phase) 由最具体的元素（触发节点）然后逐渐向上传播到最不具体的那个节点，也就是DOM中最高层的父节点

addEventListener  .onclick  onclick

## 14.typeof与instanceof
typeof与instanceof都是判断数据类型的方法，区别如下：

typeof会返回一个变量的基本类型，instanceof返回的是一个布尔值

instanceof 可以准确地判断复杂引用数据类型，但是不能正确判断基础数据类型

而typeof 也存在弊端，它虽然可以判断基础数据类型（null 除外），但是引用数据类型中，除了function 类型以外，其他的也无法判断

可以看到，上述两种方法都有弊端，并不能满足所有场景的需求

如果需要通用检测数据类型，可以采用Object.prototype.toString，调用该方法，统一返回格式“[object Xxx]”的字符串

## 15事件委托
事件代理，俗地来讲，就是把一个元素响应事件（click、keydown......）的函数委托到另一个元素

前面讲到，事件流的都会经过三个阶段： 捕获阶段 -> 目标阶段 -> 冒泡阶段，而事件委托就是在冒泡阶段完成

事件委托，会把一个或者一组元素的事件委托到它的父层或者更外层元素上，真正绑定事件的是外层元素，而不是目标元素

focus、blur这些事件没有事件冒泡机制，所以无法进行委托绑定事件

mousemove、mouseout这样的事件，虽然有事件冒泡，但是只能不断通过位置去计算定位，对性能消耗高，因此也是不适合于事件委托的

## 16.new
从上面介绍中，我们可以看到new关键字主要做了以下的工作：

创建一个新的对象obj

将对象与构建函数通过原型链连接起来

将构建函数中的this绑定到新建的对象obj上

根据构建函数返回类型作判断，如果是原始值则被忽略，如果是返回对象，需要正常处理

## 17.ajax
创建 Ajax的核心对象 XMLHttpRequest对象

通过 XMLHttpRequest 对象的 open() 方法与服务端建立连接

构建请求所需的数据内容，并通过XMLHttpRequest 对象的 send() 方法发送给服务器端

通过 XMLHttpRequest 对象提供的 onreadystatechange 事件监听服务器端你的通信状态

接受并处理服务端向客户端响应的数据结果

将处理结果更新到 HTML页面中

## 18.从上面可以看到，apply、call、bind三者的区别在于：

三者都可以改变函数的this对象指向
三者第一个参数都是this要指向的对象，如果如果没有这个参数或参数为undefined或null，则默认指向全局window
三者都可以传参，但是apply是数组，而call是参数列表，且apply和call是一次性传入参数，而bind可以分为多次传入
bind是返回绑定this之后的函数，apply、call 则是立即执行

## 19.事件循环
首先，JavaScript是一门单线程的语言，意味着同一时间内只能做一件事，但是这并不意味着单线程就是阻塞，而实现单线程非阻塞的方法就是事件循环
同步任务：立即执行的任务，同步任务一般会直接进入到主线程中执行

异步任务：异步执行的任务，比如ajax网络请求，setTimeout定时函数等
同步任务进入主线程，即主执行栈，异步任务进入任务队列，主线程内的任务执行完毕为空，会去任务队列读取对应的任务，推入主线程执行。上述过程的不断重复就事件循环

一个需要异步执行的函数，执行时机是在主函数执行结束之后、当前宏任务结束之前

先执行微任务 再执行宏任务
### 常见的微任务有：

Promise.then

MutaionObserver

Object.observe（已废弃；Proxy 对象替代）

process.nextTick（Node.js）

宏任务的时间粒度比较大，执行的时间间隔是不能精确控制的，对一些高实时性的需求就不太符合

### 常见的宏任务有：

script (可以理解为外层同步代码)
setTimeout/setInterval
UI rendering/UI事件
postMessage、MessageChannel
setImmediate、I/O（Node.js）

## 20.BOM
浏览器的全部内容可以看成DOM，整个浏览器可以看成BOM

### window
moveBy(x,y)：从当前位置水平移动窗体x个像素，垂直移动窗体y个像素，x为负数，将向左移动窗体，y为负数，将向上移动窗体
moveTo(x,y)：移动窗体左上角到相对于屏幕左上角的(x,y)点
resizeBy(w,h)：相对窗体当前的大小，宽度调整w个像素，高度调整h个像素。如果参数为负值，将缩小窗体，反之扩大窗体
resizeTo(w,h)：把窗体宽度调整为w个像素，高度调整为h个像素
scrollTo(x,y)：如果有滚动条，将横向滚动条移动到相对于窗体宽度为x个像素的位置，将纵向滚动条移动到相对于窗体高度为y个像素的位置
scrollBy(x,y)： 如果有滚动条，将横向滚动条向左移动x个像素，将纵向滚动条向下移动y个像素

### location
hash	"#contents"	utl中#后面的字符，没有则返回空串
host	www.wrox.com:80	服务器名称和端口号
hostname	www.wrox.com	域名，不带端口号
href	http://www.wrox.com:80/WileyCDA/?q=javascript#contents	完整url
pathname	"/WileyCDA/"	服务器下面的文件路径
port	80	url的端口号，没有则为空
protocol	http:	使用的协议
search	?q=javascript	url的查询字符串，通常为？后面的内容

### navigator
对象主要用来获取浏览器的属性，区分浏览器类型

### screen
浏览器窗口外面的客户端显示器的信息，比如像素宽度和像素高度

### history
history对象主要用来操作浏览器URL的历史记录，可以通过参数向前，向后，或者向指定URL跳转

## 21.内存泄漏
内存泄漏（Memory leak）是在计算机科学中，由于疏忽或错误造成程序未能释放已经不再使用的内存
Javascript 具有自动垃圾回收机制（GC：Garbage Collecation），也就是说，执行环境会负责管理代码执行过程中使用的内存

原理：垃圾收集器会定期（周期性）找出那些不在继续使用的变量，然后释放其内存

通常情况下有两种实现方式：

标记清除  循环去掉标记  标记的回收 
引用计数 加减  为0回收

- 定时器
- 闭包
- DOM对象

## 22.本地存储
- cookie
指某些网站为了辨别用户身份而储存在用户本地终端上的数据
一般不超过 4KB 的小型文本数据，它由一个名称（Name）、一个值（Value）和其它几个用于控制 cookie有效期、安全性、使用范围的可选属性组成

- sessionStorage
一旦页面（会话）关闭，sessionStorage 将会删除数据

- localStorage

生命周期：持久化的本地存储，除非主动删除数据，否则数据是永远不会过期的
存储的信息在同一域中是共享的
当本页操作（新增、修改、删除）了localStorage的时候，本页面不会触发storage事件,但是别的页面会触发storage事件。
大小：5M（跟浏览器厂商有关系）
localStorage本质上是对字符串的读取，如果存储内容多的话会消耗内存空间，会导致页面变卡
受同源策略的限制

- indexedDB

储存量理论上没有上限
所有操作都是异步的，相比 LocalStorage 同步操作性能更高，尤其是数据量较大时
原生支持储存JS的对象
是个正经的数据库，意味着数据库能干的事它都能干



## 23.函数式编程
函数式编程更加强调程序执行的结果而非执行的过程，倡导利用若干简单的执行单元让计算结果不断渐进，逐层推导复杂的运算，而非设计一个复杂的执行过程

命令式编程，声明式编程和函数式编程

// 命令式编程
var array = [0, 1, 2, 3]
for(let i = 0; i < array.length; i++) {
    array[i] = Math.pow(array[i], 2)
}

// 函数式方式
[0, 1, 2, 3].map(num => Math.pow(num, 2))

## 24.函数缓存
函数缓存，就是将函数运算过的结果进行缓存

本质上就是用空间（缓存存储）换时间（计算过程）

常用于缓存数据计算结果和缓存对象

实现函数缓存主要依靠闭包、柯里化、高阶函数，这里再简单复习下：
const memoize = function (func, content) {
  let cache = Object.create(null)
  content = content || this
  return (...key) => {
    if (!cache[key]) {
      cache[key] = func.apply(content, key)
    }
    return cache[key]
  }
}

## 25.节流 防抖
- 节流 重复操作重新计时延迟。
- 函数防抖关注一定时间连续触发的事件，只在最后执行一次，而函数节流一段时间内只执行一次

防抖在连续的事件，只需触发一次回调的场景有：

搜索框搜索输入。只需用户最后一次输入完，再发送请求
手机号、邮箱验证输入检测
窗口大小resize。只需窗口调整完成后，计算窗口大小。防止重复渲染。

节流在间隔一段时间执行一次回调的场景有：
滚动加载，加载更多或滚到底部监听
搜索框，搜索联想功能

## 26.可视区域即我们浏览网页的设备肉眼可见的区域
判断一个元素是否在可视区域，我们常用的有三种办法：

- offsetTop、scrollTop
el.offsetTop - document.documentElement.scrollTop <= viewPortHeight

<!-- 上拉加载 -->
<!-- scrollTop + clientHeight >= scrollHeight -->

getBoundingClientRect

Intersection Observer

## 27.大文件上传如何做断点续传
服务器处理数据的能力
请求超时
网络波动

将需要上传的文件按照一定的分割规则，分割成相同大小的数据块；
初始化一个分片上传任务，返回本次分片上传唯一标识；
按照一定的策略（串行或并行）发送各个分片数据块；
发送完成后，服务端根据判断数据上传是否完整，如果完整，则进行数据块合成得到原始文件

## 28.单点登录 SSO
SSO 一般都需要一个独立的认证中心（passport），子系统的登录均得通过passport，子系统本身将不参与登录操作
当一个系统成功登录以后，passport将会颁发一个令牌给各个子系统，子系统可以拿着令牌会获取各自的受保护资源，为了减少频繁认证，各个子系统在被passport授权以后，会建立一个局部会话，在一定时间内可以无需再次向passport发起认证

- 同域名下的单点登录
cookie的domain属性设置为当前域的父域，并且父域的cookie会被子域所共享。path属性默认为web应用的上下文路径

利用 Cookie 的这个特点，没错，我们只需要将Cookie的domain属性设置为父域的域名（主域名），同时将 Cookie的path属性设置为根路径，将 Session ID（或 Token）保存到父域中。这样所有的子域应用就都可以访问到这个Cookie

不过这要求应用系统的域名需建立在一个共同的主域名之下，如 tieba.baidu.com 和 map.baidu.com，它们都建立在 baidu.com这个主域名之下，那么它们就可以通过这种方式来实现单点登录

- 不同域名下的单点登录

## 29.web常见的攻击方式有哪些
- XSS (Cross Site Scripting) 跨站脚本攻击 
允许攻击者将恶意代码植入到提供给其它用户使用的页面中

存储型 提交恶意代码 存到数据库

反射型  攻击者构造出特殊的 URL，其中包含恶意代码
用户打开带有恶意代码的 URL 时，网站服务端将恶意代码从 URL 中取出，拼接在 HTML 中返回给浏览器
用户浏览器接收到响应后解析执行，混在其中的恶意代码也被执行

DOM 型
取出和执行恶意代码由浏览器端完成

攻击者提交而恶意代码
浏览器执行恶意代码

CSRF（Cross-site request forgery）跨站请求伪造
攻击者诱导受害者进入第三方网站，在第三方网站中，向被攻击网站发送跨站请求
利用受害者在被攻击网站已经获取的注册凭证，绕过后台的用户验证，达到冒充用户对被攻击的网

受害者登录a.com，并保留了登录凭证（Cookie）
攻击者引诱受害者访问了b.com
b.com 向 a.com 发送了一个请求：a.com/act=xx。浏览器会默认携带a.com的Cookie
a.com接收到请求后，对请求进行验证，并确认是受害者的凭证，误以为是受害者自己发送的请求
a.com以受害者的名义执行了act=xx
攻击完成，攻击者在受害者不知情的情况下，冒充受害者，让a.com执行了自己定义的操作

攻击一般发起在第三方网站，而不是被攻击的网站。被攻击的网站无法防止攻击发生

防止csrf常用方案如下：

阻止不明外域的访问
同源检测
Samesite Cookie
提交时要求附加本域才能获取的信息
CSRF Token
双重Cookie验证

用户打开页面的时候，服务器需要给这个用户生成一个Token
对于GET请求，Token将附在请求地址之后。对于 POST 请求来说，要在 form 的最后加上
<input type=”hidden” name=”csrftoken” value=”tokenvalue”/>
当用户从客户端得到了Token，再次提交给服务器的时候，服务器需要判断Token的有效性

SQL注入攻击
Sql 注入攻击，是通过将恶意的 Sql查询或添加语句插入到应用的输入参数中，再在后台 Sql服务器上解析执行进行的攻击

找出SQL漏洞的注入点

判断数据库的类型以及版本

猜解用户名和密码

利用工具查找Web后台管理入口

入侵和破坏

预防方式如下：

严格检查输入变量的类型和格式
过滤和转义特殊字符
对访问数据库的Web应用程序采用Web应用防火墙
