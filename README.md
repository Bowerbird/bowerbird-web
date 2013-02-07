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


Anatomy of the source code directory
====================================
the /Build directory contains the build batch file, the msbuild file, the javascript combine and minification scripts and some instructions

the /Docs directory contains the following folders:
* RavenDBConfig: RavenDB web.config with the recommented hosted ravenDb config settings
* MediaAssets: UI design documents as illustrator and photoshop files
* SpeciesData: The text files in this document are consumed by the system startup process on initial site load as explained below in 'First Run'. The system startup code is dependent on the structure of these docs.
* UML: Visio files documenting the specifications of the system in it's early stages - mostly out of date or irrelevant
* Whiteboards: Diagrams of brainstorming sessions at the whiteboard. Possibly very interesting, possibly illegible. Worth a look anyway.

the /Src directory contains the following folders:
* Bowerbird.Core: the models, commands, handlers, indexes, queries, configuration, events, event handlers, services, utilities, validators, view models and factories
* Bowerbird.Test: Legacy unit test project using NUnit. This will break the build if used so should be ignored. Feel free to use and contribute. It has some interesting implementations of using RavenDB in memory.
* Bowerbird.Web: the ASP.Net MVC3 web layer infrastructure and implementations
* Bowerbird.Website: The Client UI Moustache templates, Bowerbird backbone.marionette based javascript framework, 3rd party Javascript libraries

Additional artefacts of the build process are the folders:
/Lib: for additional dependent assemblies and build process executables
/packages: repository for NuGet packages which are pulled in as required on project compilation
/Release: repository for folder and zip file artefacts of running the build script. Copy the zip file build from this directory to your website to deploy.


Web.Config
==========
You will need to edit the web.config file where it is commented with: <!--YOU WILL NEED TO CHANGE THESE SETTINGS-->
* documentStore: databaseName refers to the RavenDB Tenant. If single tenant, leave blank. url is the path to the ravenDB instance. RavenDB in itself is a big learning curve. Checkout the docs and jabbR user groups as referenced on the www.ravendb.net site.
* environment: the url to the root of the site
* media: the relative path to the user uploaded resources. If you are serving content from a CDN, this will require a code refactor as this setting is currently relative
* species: the relative path to the source files for populating the system with species data. This is used once on initial application startup.
* If you like to leave your web.config in place when deploying or deploy it from a separate location, be sure to increment the "StaticContentIncrement" value. The build prcess increments this number to "YYYYDDMM-hhmm" and the system appends this value to all javascript resources as a cache-busting fix. If you are experiencing Javascript errors after deployment, you may have forgotten to increment this value.


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
* You have an instance of RavenDB you can point your BowerBird instance to. It can be ran in memory or hosted in IIS. For help with ravenDB and best practices, go to www.ravendb.net and the user group forum at https://jabbr.net/#/rooms/RavenDB.


First Run
=========
Bowerbird has over 200,000 species that need to be loaded and indexed. This will happen on the first load as part of the system setup. This may take a long time.
* When running in Debug mode, the species import will be limited to the _testImportLimit property in Bowerbird.Core.Config.SetupSystem.
* When running in DebugProd or DebugRelease mode, all species will be imported.
* You can see the status of the indexing by browsing to your RavenDB instance (at your local address or localhost:8080 if running in memory) and clicking on 'Stale Indexes' in the footer of the home page.

How this happens:
* When IIS starts up the app pool and the site is hit for the first time, the Web activator along with the interrogation of the document database determines if the species data needs loading and indexing. 
* This results in the creation of a Document called AppRoot in the RavenDB store.
* As the procedural code in SystemSetup works through, services are turned off and on depending on the need for events to fire.


Issues
======
If you have any issues or feature requests or improvements for this document, submit them to https://github.com/Bowerbird/bowerbird-web/issues


Unit Tests
==========
There is a long forgotten Test project but this has long since been abandoned so needs to be ignored for your build, however, Feel free to contribute to resurrect it.