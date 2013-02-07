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


Web.Config
==========
You will need to edit the web.config file where it is commented with: <!--YOU WILL NEED TO CHANGE THESE SETTINGS-->
* documentStore: databaseName refers to the RavenDB Tenant. If single tenant, leave blank. url is the path to the ravenDB instance.
* environment: the url to the root of the site
* media: the relative path to the user uploaded resources. If you are serving content from a CDN, this will require a code refactor as this setting is currently relative
* species: the relative path to the source files for populating the system with species data. This is used once on initial application startup.


Build Script
============
There is a build script at /Build/build.bat
* For command switch arguments and directions on use, read /Build/getting-build-to-work.txt
* If this build script runs successfully, you are in a state to develop and deploy your own version of BowerBird
* It is advised that you deploy using the Build script, at least the first time - not by doing a Visual Studio Publish.


Running Locally
===============
To build and run the source code for BowerBird on your development machine you will need to ensure:
* You have the NUGet Package Manager installed: http://nuget.org/
* If running locally in IIS, you will need a dedicated application pool running in integrated mode with Framework version 4.0 to host the website and another to host RavenDB
* You have an instance of RavenDB you can point your BowerBird instance to. For help with ravenDB go to www.ravendb.net. It can be ran in memory or hosted in IIS.


First Run
=========
Bowerbird has over 200,000 species that need to be loaded and indexed. This may take a long time. 
* When running in Debug mode, the species import will be limited to the _testImportLimit property in Bowerbird.Core.Config.SetupSystem.
* When running in DebugProd or DebugRelease mode, all species will be imported.
* You can see the status of the indexing by browsing to your RavenDB instance (at your local address or localhost:8080 if running in memory) and clicking on 'Stale Indexes' in the footer of the home page.


Issues
======
If you have any issues or feature requests or improvements for this document, submit them to https://github.com/Bowerbird/bowerbird-web/issues


Unit Tests
==========
There is a long forgotten Test project but this has long since been abandoned so needs to be ignored for your build, however, Feel free to contribute to resurrect it.