# TaskManager
Клиент-серверное приложение для управления проектами. Приложение было создано в процессе изучения работы:
1. WPF
2. ASP.NET Core
3. Entity Framework
4. MS SQL Server
5. Паттерна MVVM
6. PRISM
7. LINQ

## 📝Описание
Функционал приложения позволяет руководителю раздавать персонализированные и групповые задачи, разделять их на этапы, контролировать выполнение всех заданных процессов.
Приложение основано на цепочке *проект-доска-задача*.

### Виды пользователей

Для работы с проектом и его элементами предусмотрено 3 вида пользователей:
* `Admin` - администратор, имеющий все полномочия.
* `Editor` - пользователь, имеющий полномочия на создания/редактирования проектов, досок и задач. Добавления пользователей в свои проекты.
* `User` - рядовой пользователей.

### 🧭Навигация
Из главного окна пользователям доступно:
1. Переключение на вкладку `My Info` с отображением информации о текущем пользователе: фотография, роль, имя, фамилия, email, телефон (при наличии)<br>
<div " align="center">
  
![Вкладка "My Info"](https://github.com/user-attachments/assets/d1d6cd8b-8e6f-4ad1-9df3-39933af3cb47)

</div>

2. Переключение на вкладку `My Projects` с отображением информации о видимых проектах <br>
<div " align="center">
  
![TaskManager_MyProjects](https://github.com/user-attachments/assets/7268a06d-c10e-450d-958f-ddec5a1267bd)

</div>

3. Переключение на вкладку `My Desks` с отображением информации о доступных досках <br>
<div " align="center">
  
![TaskManager_MyDesks](https://github.com/user-attachments/assets/16b55b8f-2141-4836-b3f0-e96126781849)

</div>

4. Переключение на вкладку `My Tasks` с отображением информации о доступных задачах <br>
<div " align="center">
  
![TaskManager_MyTasks](https://github.com/user-attachments/assets/1476d2d6-5afb-43bb-a8a4-8a83cf9184a4)

</div>

5. Переключение на вкладку `Users` с отображение информации о пользователях (вкладка доступна и видна только администратору `Admin`) <br>
<div " align="center">
  
![TaskManager_Users](https://github.com/user-attachments/assets/ce566f82-3864-4a1f-86a4-288158bec04f)

</div>

6. Выход с помощью кнопки `Logout`

### 🧩Структура проекта

<div " align="center">

![СтруктураПроекта](https://github.com/user-attachments/assets/93eb3196-4008-4513-a1de-b9bd85a2eb0d)

</div>

### 💼Создание проекта
Для создания проекта пользователям `Admin` и `Editor` необходимо перейти по пути `My projects -> New project`

<div " align="center">
  
https://github.com/user-attachments/assets/3a69ad1c-7aa0-476f-931d-e7a24b45021b
  
</div>

### Добавление и удаление пользователей из проекта

Добавление пользователей в проект происходит в окне проекта при нажатии на кнопку `Add users`.<br>
Удаление пользователя происходит с помощью контекстного меню.

<div " align="center">

https://github.com/user-attachments/assets/9c00b74e-5631-40f0-859d-5453e1e247dd
 
</div>

### Создание досок

Для создания досок в проекте необходимо перейти на страницу досок с помощью кнопки `Desks` соответвующего проекта.<br>
***Примечание:*** доски могут быть приватными и видны только создавшему их пользователю.

<div " align="center">

https://github.com/user-attachments/assets/affd1320-efda-4339-8152-8a6dc9a0145f
 
</div>

### Создание задач

Для создания задач необходимо перейти на страницу соответсвующей доски.<br>
Задачи могут находиться в одном из состояний согласно выбранными колонками доски.<br>
Для изменения состояния одной задачи необходимо "перенести" задачу с текущей колонки на желаемую.<br>

<div " align="center">

https://github.com/user-attachments/assets/e6403ae6-a6f2-44fa-839c-a6cc11f314be
 
</div>

### Создание нового пользователя

Создание пользователей доступно достолько администраторам `Admin`.<br>

<div " align="center">

https://github.com/user-attachments/assets/e9ac5745-7e5c-4724-9324-b6262cf8cc8a
 
</div>

Имеется возможность пакетного добавления пользователей через таблицу `Excel`

<div " align="center">

https://github.com/user-attachments/assets/54566cc2-b2ab-49ec-adc1-31bca1c1e986
 
</div>

## 🛠️Техническая часть

Решение реализовано в виде **клиент-серверного приложения**:
* Клиентская часть реализована на `WPF` на основе маттерна `MVVM`
* Серверная часть реализована на `Web API` на `ASP.NET CORE`

Решение состоит из 4 частей:
1. `TaskManager.API` - WEB API на ASP.NET CORE.
2. `TaskManager.Client` - клиентская часть на WPF
3. `TaskManager.ClientTests` - проект Unit-тестов для клиентской части
4. `TaskManager.Common.Models` - проект библиотеки классов, содержащий общие модели для клиентской и серверной части. В данной библиотеке находится сервис для валидации данных

Все объекты приложения хранятся в БД `Microsoft SQL Server`. Взаимодействие с БД реализовано через `Entity Framework` (EF). <br>
При запуске приложения проверяется, существуют ли роль `Admin`. При необходимости, создаётся новый пользователь с данной ролью.

---
> [!NOTE]
> Данные администратора по умолчанию:\
*Логин:* `admin@admin.com`\
*Пароль:* `qwerty123`
---

### 📚Используемые библиотеки 

* Microsoft.AspNetCore.Authentication.JwtBearer
* Microsoft.EntityFrameworkCore.SqlServer
* Newtonsoft.Json
* ClosedXML
* Prism (Prism.Core, Prism.Wpf)

### 🏗️Архитектура

Структура каталога решения:<br>

<div " align="center">

  ![image](https://github.com/user-attachments/assets/a2406ac4-52a9-4873-8441-9a27c769ffbc)
  ![image](https://github.com/user-attachments/assets/f276d7d1-72e8-42b2-95cf-a57822deb936)
  ![image](https://github.com/user-attachments/assets/d7737db9-bb7a-45ff-95bf-8ed3087e4d9a)
  
</div>
