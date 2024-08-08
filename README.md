# TaskManager
Клиент-серверное приложение для управления проектами. Приложение было создано в процессе изучения работы:
1. WPF
2. ASP.NET Core
3. Entity Framework
4. MS SQL Server
5. Паттерна MVVM
6. PRISM
7. LINQ

## Описание работы
Функционал приложения позволяет руководителю раздавать персонализированные и групповые задачи, разделять их на этапы, контролировать выполнение всех заданных процессов.
Приложение основано на цепочке *проект-доска-задача*.

### Структура проекта

<div " align="center">

![СтруктураПроекта](https://github.com/user-attachments/assets/93eb3196-4008-4513-a1de-b9bd85a2eb0d)

</div>

### Виды пользователей

Для работы с проектом и его элементами предусмотрено 3 вида пользователей:
* `Admin` - администратор, имеющий все полномочия.
* `Editor` - пользователь, имеющий полномочия на создания/редактирования проектов, досок и задач. Добавления пользователей в свои проекты.
* `User` - рядовой пользователей.

### Создание проекта
Для создание проекта пользователям `Admin` и `Editor` необходимо перейти по пути <ins>My projects -> New project</ins>

<div " align="center">
  
https://github.com/user-attachments/assets/3a69ad1c-7aa0-476f-931d-e7a24b45021b
  
</div>

### Добавление и удаление пользователей из проекта

Добавление пользователей в проект происходит в окне проекти при нажатии на кнопку `Add users`.
Удаление пользователя происходит с помощью контекстного меню.

<div " align="center">

https://github.com/user-attachments/assets/9c00b74e-5631-40f0-859d-5453e1e247dd
 
</div>

### Создание досок

Создание досок в проекте необходимо перейте на страницу досок с помощью кнопки `Desks` соответвующего проекта.
***Примечание:*** доски могут быть приватными и видны только создавшему их пользователю.

<div " align="center">

https://github.com/user-attachments/assets/affd1320-efda-4339-8152-8a6dc9a0145f
 
</div>

### Создание задач

Для создания задач необходимо перейте на страницу соответсвующей доски.
Задачи могут находиться в одном из состояний согласно выбранными колонками доски.
Для изменения состояния одной задачи необходимо "перенести" задачу с текущей колонки на желаемую.

<div " align="center">

https://github.com/user-attachments/assets/e6403ae6-a6f2-44fa-839c-a6cc11f314be
 
</div>
