AwsQueue
========

This utility uses Queuing Theory to predict maximum server load for AWS EC2 and RDS instances.

This project is written in C# .NET and requires Visual Studio for compilation. The end result is a console app.

All parameters are configurable from within Program.cs

Program flow and logic are illustrated in the repo as PoissonQueuing.pdf

Things to do:
- Convert console app to and interactive app
- Accept input for 
    - average service time
    - simulation time
    - requests per month
    - number of servers
    - simultaneous connections
    - arrival rate