# Semester Planner

## Description
This application was created to ease the students academic year by helping them track all their modules and record how many hours a week they should be self-studying.


## Motivation
As a busy student myself I have often found that I lose track of time and sometimes the weeks pass me by and I have been falling behind. With the help of this application I will be able to keep up to date with my work and make steady improvements to my understanding of my work each week through the self-study hours. 

## Features

- Adding semester info such as the start date and number of weeks. This can only be added once by the user when they login for the first time.
- Adding the module info such as the module code, name, number of credits, the week day in which you want to study that module and amount of class hours in the week. The amount of self-study hours you need to complete is then also calculated and displayed to you based on the information you inputted.
- Being able to track the amount of self-study hours you do and be given the remaining amount for the week. 
- You are able to view all your modules and the required self-study hours for the week as wells your remaining hours.
- Being able to receive reminders to work on a module that has been pre-set for that week day.

## Security Features
We have updated the application and implemented a register/login system. This will help keep a record of all users on the system. We have also put precautions in place that only allow the registered user is able to view their own data ensuring that all data is secured to each user.

## Installation

Clone this repository to your local machine using git clone: https://github.com/VCDBN/prog6212-poe-simone-eve.git
2. Open the application in Visual Studio (Downloadable at: https://visualstudio.microsoft.com/downloads/).
3. Ensure you are using .NET 6.
4. Run the application by pressing the green play button.

## Usage

When the web app is entered you are created with a warm welcome page. If you direct your eyes to the top of the page you will find a navigation bar. This nav bar contains the following: 
On the right there are two tabs, “Login” and “Register”. As a new user you will select “Register” in order to register as a new user to the system. Once registered you will have access to all the other tabs. If you are a pre-existing user you will select the “Login” tab. This tab will ask you to enter the necessary information and once entered you will be logged into the system. Once logged in the “Login” tab will change to a “Logout” tab where you can click to logout of the system.

On the left-hand size of the nav bar you will have the following options:
Home - This will take you to the home page. 
Modules - Once clicked this tab has two options, “Create Module” and “View Modules”. To create a new module you will click “Create Module”, thereafter you will be taken to a page where you can enter module code, name, number of credits, the week day in which you want to study that module and amount of class hours in the week. All this information will then be saved into the database. To view all the information saved you will press “View Modules”, this will the direct you to a page which allows you to view all your modules and all of your data associated. 
Semester - This tab will allow you to view your semester details and edit or delete the data.
Track Self Study Hours - This tab will allow you to track the amount of hours you have spent on a specific module on a specific day. This number will then update the amount of hours you have for said module in the database so you are always informed of how many hours you have left.

## Changelog
There are no changes to be made at this time.

## Demo
A demo of the application can be viewed at: https://youtu.be/ge5JzEdOZAQ

## Contributing

If you would like to contribute to this application you are more than welcome. Create a pull request but before pushing into the branch please ensure there are no bugs in your code or anything else that could endanger the application. Additionally, three SQL tables were created, if you would like to access the application you will need to create them. They are as follows: 

create table Registered_User(
users_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
username varchar(250) NOT NULL,
password varchar(250) NOT NULL
);

create table semester(
semester_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
semester_weeks int NOT NULL,
semester_date DATE NOT NULL,
users_id int not null,
FOREIGN KEY(users_id) REFERENCES Registered_User(users_id)
);

create table module(
module_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
module_code varchar(20) NOT NULL,
module_name varchar(40) NOT NULL,
number_of_credits int NOT NULL,
weekly_hours int NOT NULL,
self_study_hours int NOT NULL,
total_self_study_hours int NOT NULL,
users_id int not null,
FOREIGN KEY(users_id) REFERENCES Registered_User(users_id),
);

create table SelfStudy_Tracker(
tracker_id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
moduleName varchar(30) NOT NULL,
studyHours int NOT NULL,
currentDate DateTime NOT NULL
);


## Credits

I would like to credit the “Pro C 9 with .NET 5 Foundational Principles and Practices in Programming” textbook written by Andrew Troelsen and Phillip Japikse which helped enable me to complete this application.




