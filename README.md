Project Description
===================
BowerBird is:
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
* Microsoft Windows 64 bit Environment running .Net Framework version 4
* Visual Studio 2010 installed with MVC3 for web based development
* Node.js installed and added to the system PATH variable
* MSBuildTasks - Download and install the MSI from this project: https://github.com/loresoft/msbuildtasks
* Use git (recommended) or simply downloading source as a zip file to your environment


Anatomy of the source code directory
====================================
the /Build directory contains the build batch file, the msbuild file, the javascript combine and minification scripts and some instructions:
The combination of files in this directory combine and minify the javascript according to the build flag levels, execute builds of the source, copy to a date-deployconfig named Release directory folder and zip said folder for distribution

the /Docs directory contains the following folders:
* RavenDBConfig: RavenDB web.config with the recommented hosted ravenDb config settings
* MediaAssets: UI design documents as illustrator and photoshop files
* SpeciesData: The text files in this document are consumed by the system startup process on initial site load as explained below in 'First Run'. The system startup code is dependent on the structure of these docs
* UML: Visio files documenting the specifications of the system in it's early stages - mostly out of date or irrelevant
* Whiteboards: Diagrams of brainstorming sessions at the whiteboard. Possibly very interesting, possibly illegible. Worth a look anyway

the /Src directory contains the following folders:
* Bowerbird.Core: the models, commands, handlers, indexes, queries, configuration, events, event handlers, services, utilities, validators, view models and factories
* Bowerbird.Test: Legacy unit test project using NUnit. This will break the build if used so should be ignored. Feel free to use and contribute. It has some interesting implementations of using RavenDB in memory
* Bowerbird.Web: the ASP.Net MVC3 web layer infrastructure and implementations
* Bowerbird.Website: The Client UI Moustache templates, Bowerbird backbone.marionette based javascript framework, 3rd party Javascript libraries

the /Lib: for additional dependent assemblies and build process executables
* Nustache.*.* is an assembly that allows serverside use of client side Mustache templates. This impementation is a customised version of the Nustache code base with some logic improvements. Source code is hosted at [not uploaded yet - ask frank@radocaj.com for a copy]
* hubify.exe is a custom build of a SignalR tool, particularly the file: SignalR.ProxyGenerator.Program.cs. The custom build applies an AMD compliant wrapper to the resultant signalR javascript build artefact. The source for this forked code can be found, downloaded and compile from https://github.com/hamishcrittenden/SignalR/ If you download and compile this source code, DO NOT USE ANY ASSEMBLY OR EXECUTABLE OTHER THAN hubify.exe. VERY IMPORTANT!!!
* AjaxMin.*.* is a minification assembly that minifies javascript files

Additional artefacts of the build process are the folders:
/packages: repository for NuGet packages which are pulled in as required on project compilation
/Release: repository for folder and zip file artefacts of running the build script. Copy the zip file build from this directory to your website to deploy


Web.Config
==========
You will need to edit the web.config file where it is commented with: <!--YOU WILL NEED TO CHANGE THESE SETTINGS-->
* documentStore: databaseName refers to the RavenDB Tenant. If single tenant, leave blank. url is the path to the ravenDB instance. RavenDB in itself is a big learning curve. Checkout the docs and jabbR user groups as referenced on the www.ravendb.net site
* environment: the url to the root of the site
* media: the relative path to the user uploaded resources. If you are serving content from a CDN, this will require a code refactor as this setting is currently relative
* species: the relative path to the source files for populating the system with species data. This is used once on initial application startup
* If you like to leave your web.config in place when deploying or deploy it from a separate location, be sure to increment the "StaticContentIncrement" value. The build prcess increments this number to "YYYYDDMM-hhmm" and the system appends this value to all javascript resources as a cache-busting fix. If you are experiencing Javascript errors after deployment, you may have forgotten to increment this value


Build Script
============
There is a build script at /Build/build.bat
* For command switch arguments and directions on use, read /Build/getting-build-to-work.txt
* If this build script runs successfully, you are in a state to develop and deploy your own version of BowerBird
* It is advised that you deploy using the Build script, at least the first time - not by doing a Visual Studio Publish


Running Locally
===============
To build and run the source code for BowerBird on your development machine you will need to ensure:
* You have the NUGet Package Manager installed: http://nuget.org/
* If running locally in IIS, you will need a dedicated application pool running in integrated mode with Framework version 4.0 to host the website and another to host RavenDB
* You have an instance of RavenDB you can point your BowerBird instance to. It can be ran in memory or hosted in IIS. For help with ravenDB and best practices, go to www.ravendb.net and the user group forum at https://jabbr.net/#/rooms/RavenDB


First Run
=========
BowerBird has over 200,000 species that need to be loaded and indexed. This will happen on the first load as part of the system setup. This may take a long time
* When running in Debug mode, the species import will be limited to the _testImportLimit property in Bowerbird.Core.Config.SetupSystem
* When running in DebugProd or DebugRelease mode, all species will be imported
* You can see the status of the indexing by browsing to your RavenDB instance (at your local address or localhost:8080 if running in memory) and clicking on 'Stale Indexes' in the footer of the home page

How this happens:
* When IIS starts up the app pool and the site is hit for the first time, the Web activator along with the interrogation of the document database determines if the species data needs loading and indexing. 
* This results in the creation of a Document called AppRoot in the RavenDB store
* As the procedural code in SystemSetup works through, services are turned off and on depending on the need for events to fire

If you have a healthy running application, the homepage will load, you will be able to register and log in. When logged in, you will see an active people online tab in the bottom left of screen with your avatar and name in it and you will not have any javascript errors


Issues
======
If you have any issues or feature requests or improvements for this document, submit them to https://github.com/bowerbird/bowerbird-web/issues


Development Schedule
====================
Current Version: 1.0.47 - Released: 12 August 2014
Next Version Scheduled for release: 15 September 2014


With Thanks To
==============
The BowerBird development team would like to thank JetBrains for their commitment to Open Source Software projects with their granting of ReSharper and TeamCity licences to help us develop BowerBird.

![ReSharper](http://www.jetbrains.com/img/logos/logo_resharper_small.gif)  
[ReSharper](http://www.jetbrains.com/resharper/) - the most advanced productivity add-in for Visual Studio!

Have Fun!
=========
Frank - frank@melbournemade.com.au
Hamish - hamish@melbournemade.com.au
Ken - kwalker@museum.vic.gov.au
