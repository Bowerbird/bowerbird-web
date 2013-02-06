Project Description
===================
Bowerbird is:

*	A social network without walls, closed doors or exclusivity; totally open, totally visible and totally collaborative
*	A continuous activity stream of natural history observations
*	A workspace tool for the Citizen Science as well as the Scientific Biodiversity and Biosecurity communities where individuals can form groups; communicate and collaborate; make, discuss and identify their observations.
*	An aggregator of observational data (specimen and images/video) and community comment into species pages with the added richness of interpretation from external sources (eg. ALA, GBIF, BHL, specimen data, mapping overlays: rainfall, climate, vegetation).


The Technology Stack
====================

* Client: Asp.Net MVC 3
* Server: .Net Framework V4.0
* Persistence: RavenDB
* UI: CSS3, JQuery, Backbone, Underscore & Mustache
* Server to Client: signalR


Getting Started
===============

It is assumed that you are familiar with IIS, the Microsoft .Net Framework 
To develop for BowerBird, you will need to ensure the following development machine configuration:
* Microsoft Windows 64 bit Environment running .Net Framework version 4.
* Visual Studio 2010 installed with MVC3 for web based development
* Node.js installed and added to the system PATH variable
* MSBuildTasks - Download and install the MSI from this project: https://github.com/loresoft/msbuildtasks


Running Locally
===============

To build and run the source code for BowerBird on your development machine you will need to ensure:
* You have the NUGet Package Manager installed: http://nuget.org/
* You have an instance of RavenDB you can point your BowerBird instance to
* If running locally in IIS, you will need a dedicated application pool running in integrated mode with Framework version 4.0 to host the website and another to host RavenDB
* You will need to edit the web.config file where it is commented with: <!--YOU WILL NEED TO CHANGE THESE SETTINGS-->


Build Script
============
There is a build script at /Build/build.bat
* For command switch arguments and directions on use, read /Build/getting-build-to-work.txt
* If this build script runs successfully, you are in a state to develop and deploy your own version of BowerBird
* If you have any trouble, submit a bug to https://github.com/Bowerbird/bowerbird-web/issues


Debugging
=========
You can run your RavenDB instance in memory from the ..... exe file shortcut with a -RAM switch (add link)


First Run
=========
Bowerbird has over 200,000 species that need to be loaded and indexed. If debugging, you can limit the loading to ... by doing ....
