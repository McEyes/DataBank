## 1.Select
SELECT select_list 
[ INTO new_table ] 
FROM table_source 
[ WHERE search_condition ] 
[ GROUP BY group_by_expression ] 
[ HAVING search_condition ] 
[ ORDER BY order_expression [ ASC | DESC ] ] 

- wherer
下面的操作符能被使用在 WHERE 中：
=,<>,>,<,>=,<=,BETWEEN,LIKE
在 WHERE 子句中 AND 和 OR 被用来连接两个或者更多的条件。

- Between…And
范围查询

- Distinct
用途：
DISTINCT 关键字被用作返回唯一的值，去重。
```sql
SELECT DISTINCT Company FROM Orders
```

- Order by 默认的方式是 ASC
```sql
SELECT column-name(s) FROM table-name ORDER BY  { order_by_expression [ ASC | DESC ] } 
```

- Group by
```sql
SELECT column,SUM(column) FROM table GROUP BY column
```

转成Company,SUM(Amount)两列显示company的sum
SELECT Company,SUM(Amount) FROM Sales
GROUP BY Company

- Having 类似where过滤
```sql
SELECT column,SUM(column) FROM table
GROUP BY column
HAVING SUM(column) condition value
```
```sql
SELECT Company,SUM(Amount) FROM Sales
GROUP BY Company HAVING SUM(Amount)>10000
```

- Join
当你要从两个或者以上的表中选取结果集时，你就会用到 JOIN。
用 Employees 的 ID 和 Orders 的 ID 相关联选取数据：
```sql
SELECT Employees.Name, Orders.Product
FROM Employees, Orders
WHERE Employees.ID = Orders.ID
```
等价
```sql
SELECT Employees.Name, Orders.Product
FROM Employees
INNER JOIN Orders
ON Employees.ID = Orders.ID
```
### (1)INNER JOIN 返回的结果集是两个表中所有相匹配的数据。

### (2)LEFT JOIN 的语法：
LEFT JOIN 返回 "first_table" 中所有的行，尽管在 "second_table" 中没有相匹配的数据。
```sql
SELECT field1, field2, field3
FROM first_table
LEFT JOIN second_table
ON first_table.keyfield = second_table.foreign_keyfield
```
用 "Employees" 表去左外联结 "Orders" 表去找出相关数据：
SELECT Employees.Name, Orders.Product
FROM Employees
LEFT JOIN Orders
ON Employees.ID = Orders.ID

### (3)RIGHT JOIN 的语法：
```sql
SELECT field1, field2, field3
FROM first_table
RIGHT JOIN second_table
ON first_table.keyfield = second_table.foreign_keyfield
```
RIGHT JOIN 返回 "second_table" 中所有的行，尽管在 "first_table" 中没有相匹配的数据

- Alias , as
别名
```sql
SELECT column AS column_alias FROM table
```
```sql
SELECT LastName AS Family, FirstName AS Name
FROM Persons
```

## 2.Insert
```sql
INSERT INTO table_name
VALUES (value1, value2,....)

INSERT INTO table_name (column1, column2,...)
VALUES (value1, value2,....)
```

## 3.Update
```sql
UPDATE table_name SET column_name = new_value
WHERE column_name = some_value
```
```sql
UPDATE Person
SET Address = 'Stien 12', City = 'Stavanger'
WHERE LastName = 'Rasmussen'
```

## 4.Delete
```sql
DELETE FROM table_name WHERE column_name = some_value
```

## 5.Create Table
```sql
CREATE TABLE table_name 
(
column_name1 data_type, 
column_name2 data_type, 
.......
)
```
```sql
CREATE TABLE Person 
(
LastName varchar(30),
FirstName varchar(30),
Address varchar(120),
Age int(3) 
)
```
数据类型	描述
integer(size)
int(size)
smallint(size)
tinyint(size)	只存储整数。数字的最大位数由括号内的 size 指定。
decimal(size,d)
numeric(size,d)	存储带有小数的数字。数字的最大位数由 "size" 指定。小数点右边的最大位数由 "d" 指定。
char(size)	存储一个固定长度的字符串（可以包含字母、数字和特殊字符）。字符串大小由括号内的 size 指定。
varchar(size)	存储一个可变长度的字符串（可以包含字母、数字和特殊字符）。字符串的最大长度由括号内的 size 指定。
date(yyyymmdd)	存储日期。

## 6.Alter Table
在已经存在的表中增加或者移除字段。
```sql
ALTER TABLE table_name 
ADD column_name datatype
ALTER TABLE table_name 
DROP COLUMN column_name
```
```sql
ALTER TABLE Person ADD City varchar(30)

ALTER TABLE Person DROP COLUMN Address
```

## 7.Drop Table
DROP TABLE table_name

## 8.Create Database
CREATE DATABASE database_name

## 9.Drop Database
DROP DATABASE database_name

## 10.聚集函数
- count 传回选取的结果集中行的数目。
```sql
SELECT COUNT(column_name) FROM table_name
```
- sum 
```sql
SELECT SUM(column_name) FROM table_name
```
- avg
```sql
SELECT AVG(column_name) FROM table_name
```

- max min
```sql
SELECT MAX(column_name) FROM table_name
```

## SQL 视图
```sql
CREATE VIEW view_name AS
SELECT column1, column2, ...
FROM table_name
WHERE condition;
```
CREATE VIEW: 声明你要创建一个视图。
view_name: 指定视图的名称。
AS: 指定关键字，表示视图的定义开始。
SELECT column1, column2, ...: 指定视图中包含的列，可以是表中的列或计算列。
FROM table_name: 指定视图从哪个表中获取数据。
WHERE condition: 可选部分，用于指定筛选条件，限制视图中的行。

```sql
-- 创建包含高工资员工信息的视图
CREATE VIEW high_salary_employees AS
SELECT employee_id, first_name, last_name, salary
FROM employees
WHERE salary > 50000;
```