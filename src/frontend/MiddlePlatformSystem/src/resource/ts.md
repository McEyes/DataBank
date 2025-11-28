## 1.ts
TypeScript 的特性主要有如下：

类型批注和编译时类型检查 ：在编译时批注变量类型
类型推断：ts 中没有批注变量类型会自动推断变量的类型
类型擦除：在编译过程中批注的内容和接口会在运行时利用工具擦除
接口：ts 中用接口来定义对象类型
枚举：用于取值被限定在一定范围内的场景
Mixin：可以接受任意类型的值
泛型编程：写代码时使用一些以后才指定的类型
名字空间：名字只在该区域内有效，其他区域可重复使用该名字而不冲突
元组：元组合并了不同类型的对象，相当于一个可以装不同类型数据的数组

## 2.数据类型
boolean（布尔类型）
number（数字类型）
string（字符串类型）
array（数组类型）
tuple（元组类型）
enum（枚举类型）
any（任意类型）
null 和 undefined 类型
void 类型
never 类型
object 对象类型

## 3.枚举
enum Direction {
    Up,   // 值默认为 0
    Down, // 值默认为 1
    Left, // 值默认为 2
    Right // 值默认为 3
}

console.log(Direction.Up === 0); // true
console.log(Direction.Down === 1); // true
console.log(Direction.Left === 2); // true
console.log(Direction.Right === 3); // true

## 3.接口
interface User {
    name: string
    age?: number
    readonly isMale: boolean
    say: (words: string) => string,
    [propName: string]: any;
}
不仅仅是上述的属性
- 类型推断
getUserName({color: 'yellow'} as User)

- 继承
interface Father {
    color: String
}

interface Mother {
    height: Number
}

interface Son extends Father,Mother{
    name: string
    age: Number
}

- 应用
// 先定义一个接口
interface IUser {
  name: string;
  age: number;
}

const getUserInfo = (user: IUser): string => {
  return `name: ${user.name}, age: ${user.age}`;
};

// 正确的调用
getUserInfo({name: "koala", age: 18});

## 4.类
JavaScript 的class依然有一些特性还没有加入，比如修饰符和抽象类

class Car {
    // 字段
    engine:string;

    // 构造函数
    constructor(engine:string) {
        this.engine = engine
    }

    // 方法
    disp():void {
        console.log("发动机为 :   "+this.engine)
    }
}

- 类的继承使用过extends的关键字
class Animal {
    move(distanceInMeters: number = 0) {
        console.log(`Animal moved ${distanceInMeters}m.`);
    }
}

class Dog extends Animal {
    bark() {
        console.log('Woof! Woof!');
    }
}

const dog = new Dog();
dog.bark();
dog.move(10);
dog.bark();

- 类可以对父类的方法重新定义，这个过程称之为方法的重写，通过super关键字是对父类的直接引用，该关键字可以引用父类的属性和方法

class PrinterClass {
   doPrint():void {
      console.log("父类的 doPrint() 方法。")
   }
}

class StringPrinter extends PrinterClass {
   doPrint():void {
      super.doPrint() // 调用父类的函数
      console.log("子类的 doPrint()方法。")
   }
}

- 修饰符
公共 public：可以自由的访问类程序里定义的成员
私有 private：只能够在该类的内部进行访问
受保护 protect：除了在该类的内部可以访问，还可以在子类中仍然可以访问
通过readonly关键字进行声明，只读属性必须在声明时或构造函数里被初始化

- 静态属性
这些属性存在于类本身上面而不是类的实例上，通过static进行定义，访问这些属性需要通过 类型.静态属性 的这种形式访问，如下所示：
class Square {
    static width = '100px'
}

console.log(Square.width) // 100px

- 抽象类
抽象类做为其它派生类的基类使用，它们一般不会直接被实例化，不同于接口，抽象类可以包含成员的实现细节
abstract class Animal {
    abstract makeSound(): void;
    move(): void {
        console.log('roaming the earch...');
    }
}

class Cat extends Animal {

    makeSound() {
        console.log('miao miao')
    }
}

const cat = new Cat()

cat.makeSound() // miao miao
cat.move() // roaming the earch...

## 5.函数

- 可选参数
const add = (a: number, b?: number) => a + (b ? b : 0)

- 剩余类型
const add = (a: number, ...rest: number[]) => rest.reduce(((a, b) => a + b), a)

- 函数重载
// 上边是声明
function add (arg1: string, arg2: string): string
function add (arg1: number, arg2: number): number
// 因为我们在下边有具体函数的实现，所以这里并不需要添加 declare 关键字

// 下边是实现
function add (arg1: string | number, arg2: string | number) {
  // 在实现上我们要注意严格判断两个参数的类型是否相等，而不能简单的写一个 arg1 + arg2
  if (typeof arg1 === 'string' && typeof arg2 === 'string') {
    return arg1 + arg2
  } else if (typeof arg1 === 'number' && typeof arg2 === 'number') {
    return arg1 + arg2
  }
}

- 和ES6区别
从定义的方式而言，typescript 声明函数需要定义参数类型或者声明返回值类型
typescript 在参数中，添加可选参数供使用者选择
typescript 增添函数重载功能，使用者只需要通过查看函数声明的方式，即可知道函数传递的参数个数以及类型

## 6.泛型
function returnItem (para: number): number {
    return para
}

function returnItem (para: string): string {
    return para
}

function returnItem<T>(para: T): T {
    return para
}
泛型通过<>的形式进行表述，可以声明：

- 函数
function swap<T, U>(tuple: [T, U]): [U, T] {
    return [tuple[1], tuple[0]];
}

swap([7, 'seven']); // ['seven', 7]

- 接口
interface ReturnItemFn<T> {
    (para: T): T
}

const returnItem: ReturnItemFn<number> = para => para

- 类
class Stack<T> {
    private arr: T[] = []

    public push(item: T) {
        this.arr.push(item)
    }

    public pop() {
        this.arr.pop()
    }
}

const stack = new Stacn<number>()

- 索引类型、约束类型
function getValue<T extends object, U extends keyof T>(obj: T, key: U) {
  return obj[key] // ok
}

- 多类型约束
interface FirstInterface {
  doSomething(): number
}

interface SecondInterface {
  doSomethingElse(): string
}

interface ChildInterface extends FirstInterface, SecondInterface {

class Demo<T extends ChildInterface> {
  private genericProperty: T

  constructor(genericProperty: T) {
    this.genericProperty = genericProperty
  }
  useT() {
    this.genericProperty.doSomething()
    this.genericProperty.doSomethingElse()
  }
}
}

## 7.高级类型
常见的高级类型有如下：

- 交叉类型
T & U
适用于对象合并场景，如下将声明一个函数，将两个对象合并成一个对象并返回
function extend<T , U>(first: T, second: U) : T & U {
    let result: <T & U> = {}
    for (let key in first) {
        result[key] = first[key]
    }
    for (let key in second) {
        if(!result.hasOwnProperty(key)) {
            result[key] = second[key]
        }
    }
    return result
}
- 联合类型
function formatCommandline(command: string[] | string) {
  let line = '';
  if (typeof command === 'string') {
    line = command.trim();
  } else {
    line = command.join(' ').trim();
  }
}
- 类型别名
type some = boolean | string
- 类型索引
keyof 类似于 Object.keys ，用于获取一个接口中 Key 的联合类型。
interface Button {
    type: string
    text: string
}

type ButtonKeys = keyof Button
// 等效于
type ButtonKeys = "type" | "text"
- 类型约束
function getValue<T, K extends keyof T>(obj: T, key: K) {
  return obj[key]
}

const obj = { a: 1 }
const a = getValue(obj, 'a')
- 映射类型
通过 in 关键字做类型的映射，遍历已有接口的 key 或者是遍历联合类型
type Readonly<T> = {
    readonly [P in keyof T]: T[P];
};

interface Obj {
  a: string
  b: string
}

type ReadOnlyObj = Readonly<Obj>
- 条件类型
T extends U ? X : Y

## 8.装饰器
@expression 的形式其实是Object.defineProperty的语法糖

expression求值后必须也是一个函数，它会在运行时被调用，被装饰的声明信息做为参数传入

类的装饰器可以装饰：

类

方法/属性

参数

访问器
可以看到，使用装饰器存在两个显著的优点：

代码可读性变强了，装饰器命名相当于一个注释
在不改变原有代码情况下，对原来功能进行扩展
后面的使用场景中，借助装饰器的特性，除了提高可读性之后，针对已经存在的类，可以通过装饰器的特性，在不改变原有代码情况下，对原来功能进行扩展

## 9.命名空间和模块区别
命名空间是位于全局命名空间下的一个普通的带有名字的 JavaScript 对象，使用起来十分容易。但就像其它的全局命名空间污染一样，它很难去识别组件之间的依赖关系，尤其是在大型的应用中

像命名空间一样，模块可以包含代码和声明。 不同的是模块可以声明它的依赖

在正常的TS项目开发过程中并不建议用命名空间，但通常在通过 d.ts 文件标记 js 库类型的时候使用命名空间，主要作用是给编译器编写代码的时候参考使用