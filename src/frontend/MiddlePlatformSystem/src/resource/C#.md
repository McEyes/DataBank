C# 是大小写敏感的。
所有的语句和表达式必须以分号（;）结尾。
程序的执行从 Main 方法开始。

## 数据类型
值类型（Value types） int、char、float 结构体是值类型数据结构
引用类型（Reference types） object、dynamic 和 string
指针类型（Pointer types） type* identifier;

声明变量
<data_type> <variable_name> = value;
声明常量
const <data_type> <constant_name> = value;

变量的作用域通常由花括号 {} 定义的代码块来确定。

## C# 支持的访问修饰符如下所示：

public：所有对象都可以访问；
private：对象本身在对象内部可以访问；
protected：只有该类对象及其子类对象可以访问
internal：同一个程序集的对象可以访问；
protected internal：访问限于当前程序集或派生自包含类的类型。

可空
< data_type> ? <variable_name> = null;

Null 合并运算符（ ?? ）
num3 = num1 ?? 5.34;      // num1 如果为空值则返回 5.34

初始化数组
datatype[] arrayName;
double[] balance = new double[10];

结构体
struct Books
{
   public string title;
   public string author;
   public string subject;
   public int book_id;
};  

## 定义方法
```c#
<Access Specifier> <Return Type> <Method Name>(Parameter List)
{
   Method Body
}
```

## 多态
是同一个行为具有多个不同表现形式或形态的能力。
函数根据参数等重载
虚方法是使用关键字 virtual 声明的。

虚方法可以在不同的继承类中有不同的实现。

对虚方法的调用是在运行时发生的。

动态多态性是通过 抽象类 和 虚方法 实现的。

```c#
 public virtual void Draw()
{
    Console.WriteLine("执行基类的画图任务");
}

// 继承后重写
 public override void Draw()
{
        Console.WriteLine("画一个长方形");
        base.Draw();
}
```

## 类的 析构函数 是类的一个特殊的成员函数，当类的对象超出范围时执行。

析构函数的名称是在类的名称前加上一个波浪形（~）作为前缀，它不返回值，也不带任何参数。

析构函数用于在结束程序（比如关闭文件、释放内存等）之前释放资源。析构函数不能继承或重载。
   ~Line() //析构函数
      {
         Console.WriteLine("对象已删除");
      }

 ##  特性（Attribute）是用于在运行时传递程序中各种元素（比如类、方法、结构、枚举、组件等）的行为信息的声明性标签。您可以通过使用特性向程序添加声明性信息。一个声明性标签是通过放置在它所应用的元素前面的方括号（[ ]）来描述的。

特性（Attribute）用于添加元数据，如编译器指令和注释、描述、方法、类等其他信息。.Net 框架提供了两种类型的特性：预定义特性和自定义特性。

## 委托
在 C# 中，委托（Delegate） 是一种类型安全的函数指针，它允许将方法作为参数传递给其他方法。