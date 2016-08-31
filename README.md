# FrankHogan_WarrantySystem
Final Project
AIM
To Design and implement a computer warranty system at the dealership level to help Frank Hogan Ltd keep Volkswagen group warranty claims data organized and recorded, while statistical information is automatically worked out and made available for reporting and helping the decision making process.

Code structure and description of each file:

MainWindow.xmal : 
                Markup file that defines user inter face layout
                MainWindow is made of three tabs controlers.1st tab to display charts (plot view controls are placed here)
                2nd tab is a claim form: Fields(texboxes and combo boxes) necessary for the insert, update, search and delere options are here.
                3rd tab is for more search options and KPI calculations.
                
MainWindow.xmal.cs:
                  Code  behind all the user interface. Event handlers and other methods are defined to define the app behaviour.
                  
DataClasses1.bdml:
                Defines all LINQ to SQL classes.
                
xxxExtended.cs: 
              Files that are named with final section Extended.cs are classes that extend classes genereted by LINQ to SQL. For example: ClaimExtended extends Claim class in DataClasses1.designer.cs, CarExtended extends Car class in DataClasses1.designer.cs,
              same for FaultExtended, MakeExtended and so on.
              
LoadDataWindow.xmal:
                  Defines UI design of child window for loading data from cvs file.
                  
LoadDataWindow.xmal.cs:
                  Defines event handlers for buttons in child windows LoadDataWindow.
                  
ClaimList.xmal:
              Defines UI for child window that displays a list of claims retrieved from database.
              
ClaimsList.xmal.cs: 
                Defines event handlers for ClaimList child windows.
                
ChartModel.cs:  
              Class that defines common properties and methods for all chat model classes to be used to create data for the different plots.
              
BarChartModel.cs:
              Class that extends the ChartModel class and define new properties and methods specific of Bar charts.
              
PieChartModel.cs: 
              Class that extends the ChartModel class and define new properties and methods specific of Pie charts.
              
LinearChartModel.cs:
              Class that extends the ChartModel class and define new properties and methods specific of Linear charts.
              
BarChart_LabourClaimed.cs:
              Class that extends the BarChartModel class and defines plot model and constructor to create data for the bar chart that breaks down the total claimed value into parts and labour claimed.
              
BarChart_NumberOfClaims.cs:
              Class that extends the BarChartModel class and defines plot model and constructor method to create data for the bar chart that breaks down the totalnumber of claims into number of claims paid, in process and cancelled.
              
LinearChart_SpeedKpi.cs:
              Class that extends the LinearChartModel class and defines plot model and constructor method to create data for the linear chart that shows the weekly claiming speed KPI between two dates.
              
LineChart_WeeklyClaiming.cs:
              Class that extends the LinearChartModel class and defines plot model and constructor method to create data for the linear chart that shows the weekly claiming activities based on number of warranty claims and claimed valued .
              
PieChart_ClaimedValue.cs:
              Class that extends the PieChartModel class and defines plot model and constructor to create data for the Pie chart that breaks down the total claimed value into value paid, in process and cancelled.
              
PieChart_MakeDistribution.cs:
              Class that extends the PieChartModel class and defines plot model and constructor to create data for the Pie chart that shows the total number of claims grouped by make.
              
PieChart_TypeDistribution.cs:
              Class that extends the PieChartModel class and defines plot model and constructor to create data for the Pie chart that shows the total number of claims grouped by type of warranty claim.
              
FrankHogan_Warranty.mdf and .log: 
              Database files.
              
App.xmal.cs and other files:
              Auto generated files by VS.
              
FrankHogan_WarrantySystem.csproj and other extensions:
              Auto generated files by VS.
              
Files with automobile make names are images files with respective logo.

All other folders are auto genereated by VS.

            
