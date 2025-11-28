# 1. ts 
## 1. var、let、const三者区别可以围绕下面五点展开：

- 变量提升
- 暂时性死区
- 块级作用域
- 重复声明
- 修改声明的变量
- 使用

## 2. 扩展运算符的应用
console.log(...[1, 2, 3])
// 1 2 3

console.log(1, ...[2, 3, 4], 5)
// 1 2 3 4 5

[...document.querySelectorAll('div')]

[...'hello']
// [ "h", "e", "l", "l", "o" ]

let { x, y, ...z } = { x: 1, y: 2, a: 3, b: 4 };

## 3.Array.of 和 from
from 类数组，set、map转换数组
of 声明数组

## 4.新增数组方法
- copyWithin()
[1, 2, 3, 4, 5].copyWithin(0, 3) // 将从 3 号位直到数组结束的成员（4 和 5），复制到从 0 号位开始的位置，结果覆盖了原来的 1 和 2
// [4, 5, 3, 4, 5] 

- fill() 填充
- flat()，flatMap()
将数组扁平化处理，返回一个新数组，对原数据没有影响；flatMap()方法还可以有第二个参数，用来绑定遍历函数里面的this

## 5.对象扩展
- 对象，方法简写，属性名和方法都可以[]变量声明

- super 指向当前对象的原型对象

- Object.is()与严格比较运算符（===）的行为基本一致；+0 === -0 //true
NaN === NaN // false
Object.is(+0, -0) // false
Object.is(NaN, NaN) // true
- Object.assign()
- Object.getOwnPropertyDescriptors()
- Object.setPrototypeOf()，
- Object.getPrototypeOf()
- Object.keys()，
- Object.values()，
- Object.entries()
- Object.fromEntries() 用于将一个键值对数组转为对象
Object.fromEntries([
  ['foo', 'bar'],
  ['baz', 42]
])
// { foo: "bar", baz: 42 }

## 6.函数扩展
- 默认值
function log(x, y = 'World') {
  console.log(x, y);
}

- length（参数长度）， name属性

- 箭头函数 
### 1、函数体内的this对象，就是定义时所在的对象，而不是使用时所在的对象
### 2.不可以当作构造函数，也就是说，不可以使用new命令，否则会抛出一个错误
### 3.不可以使用arguments对象，该对象在函数体内不存在。如果要用，可以用 rest 参数代替
### 4.不可以使用yield命令，因此箭头函数不能用作 Generator 函数

## 7.Map Set Set是一种叫做集合的数据结构，Map是一种叫做字典的数据结构

### （1）Set
keys()：返回键名的遍历器
values()：返回键值的遍历器
entries()：返回键值对的遍历器
forEach()：使用回调函数遍历每个成员

- add()

- delete()

- has()

- clear()

### （2）Map
const map = new Map();
map.set('foo', true);
keys()：返回键名的遍历器
values()：返回键值的遍历器
entries()：返回键值对的遍历器
forEach()：使用回调函数遍历每个成员

- size 属性
- set()
- get()
- has()
- delete()
- clear()

### （3）WeakSet
在API中WeakSet与Set有两个区别：
WeakSet只能成员只能是引用类型，而不能是其他类型的值
没有遍历操作的API
没有size属性

### （4）WeakMAP
只接受对象作为键名
没有遍历操作的API
没有clear清空方法

## 8.Promise
promise对象仅有三种状态

pending（进行中）
fulfilled（已成功）
rejected（已失败）

const promise = new Promise(function(resolve, reject) {
    resolve('随便什么数据');
    reject('随便什么数据');
});

resolve函数的作用是，将Promise对象的状态从“未完成”变为“成功”
reject函数的作用是，将Promise对象的状态从“未完成”变为“失败”
#实例方法

Promise构建出来的实例存在以下方法：

then()
catch()
finally()

### Promise构造函数存在以下方法：

- all()
- race() 只要p1、p2、p3之中有一个实例率先改变状态，p的状态就跟着改变

率先改变的 Promise 实例的返回值则传递给p的回调函数
- allSettled()
只有等到所有这些参数实例都返回结果，不管是fulfilled还是rejected，包装实例才会结束
- resolve()
- reject()
- try()

## 9.Generator
Generator是异步解决的一种方案，最大特点则是将异步操作同步化表达出来
function* foo(x) {
  var y = 2 * (yield (x + 1));
  var z = yield (y / 3);
  return (x + y + z);
}

var a = foo(5);
a.next() // Object{value:6, done:false}
a.next() // Object{value:NaN, done:false}
a.next() // Object{value:NaN, done:true}

回顾之前展开异步解决的方案：

回调函数
Promise 对象
generator 函数
async/await

## 10.Proxy
var proxy = new Proxy(target, handler)
target表示所要拦截的目标对象（任何类型的对象，包括原生数组，函数，甚至另一个代理））

handler通常以函数作为属性的对象，各属性中的函数分别定义了在执行各种操作时代理
var proxy = new Proxy(person, {
  get: function(target, propKey) {
    return Reflect.get(target,propKey)
  }
});

- get(target,propKey,receiver)：拦截对象属性的读取
- set(target,propKey,value,receiver)：拦截对象属性的设置
- has(target,propKey)：拦截propKey in proxy的操作，返回一个布尔值
- deleteProperty(target,propKey)：拦截delete proxy[propKey]的操作，返回一个布尔值
- ownKeys(target)：拦截Object.keys(proxy)、for...in等循环，返回一个数组
- ngetOwnPropertyDescriptor(target, propKey)：拦截Object.- - getOwnPropertyDescriptor(proxy, propKey)，返回属性的描述对象
- defineProperty(target, propKey, propDesc)：拦截Object.- defineProperty(proxy, propKey, propDesc），返回一个布尔值
- preventExtensions(target)：拦截Object.preventExtensions(proxy)，返回一个布尔值
- getPrototypeOf(target)：拦截Object.getPrototypeOf(proxy)，返回一个对象
- isExtensible(target)：拦截Object.isExtensible(proxy)，返回一个布尔值
- setPrototypeOf(target, proto)：拦截Object.setPrototypeOf(proxy, proto)，返回一个布尔值
- apply(target, object, args)：拦截 Proxy 实例作为函数调用的操作
- construct(target, args)：拦截 Proxy 实例作为构造函数调用的操作

## 11.Reflect
若需要在Proxy内部调用对象的默认行为，建议使用Reflect，其是ES6中操作对象而提供的新 API

基本特点：

只要Proxy对象具有的代理方法，Reflect对象全部具有，以静态方法的形式存在
修改某些Object方法的返回结果，让其变得更合理（定义不存在属性行为的时候不报错而是返回false）
让Object操作都变成函数行为
var proxy = new Proxy(person, {
  get: function(target, propKey) {
    return Reflect.get(target,propKey)
  }
});
var handler = {
  deleteProperty (target, key) {
    invariant(key, 'delete');
    Reflect.deleteProperty(target,key)
    return true;
  }
};

## 12.Module import export （default）

- CommonJs (典型代表：node.js早期)
- AMD (典型代表：require.js)
异步模块定义，采用异步方式加载模块。所有依赖模块的语句，都定义在一个回调函数中，等到模块加载完成之后，这个回调函数才会运行
/** main.js 入口文件/主模块 **/
// 首先用config()指定各模块路径和引用名
require.config({
  baseUrl: "js/lib",
  paths: {
    "jquery": "jquery.min",  //实际路径为js/lib/jquery.min.js
    "underscore": "underscore.min",
  }
});
// 执行基本操作
require(["jquery","underscore"],function($,_){
  // some code here
});
- CMD (典型代表：sea.js)
AMD推崇依赖前置、提前执行，CMD推崇依赖就近、延迟执行
#AMD

## 13.Decorator 装饰器

简单来讲，装饰者模式就是一种在不改变原类和使用继承的情况下，动态地扩展对象功能的设计理论。
function strong(target){
    target.AK = true
}

class soldier{ 
}

@strong
class soldier{
}

soldier.AK // true