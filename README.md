Project Description
===================
Bowerbird is:
*	A social network without walls, closed doors or exclusivity; totally open, totally visible and totally collaborative
*	A continuous activity stream of natural history observations
*	A workspace tool for the Citizen Science as well as the Scientific Biodiversity and Biosecurity communities where individuals can form groups; communicate and collaborate; make, discuss and identify their observations.
*	An aggregator of observational data (specimen and images/video) and community comment into species pages with the added richness^ of interpretation from external sources (eg. ALA, GBIF, BHL, specimen data, mapping overlays: rainfall, climate, vegetation). Well this last part (^) is on the wishlist.


The Technology Stack
====================
* Client: Asp.Net MVC 3
* Server: .Net Framework V4.0
* Persistence: RavenDB
* UI: CSS3, JQuery, Backbone, Underscore & Mustache
* Server to Client: signalR


Getting Started by Configuring your Development Environment
===========================================================
It is assumed that you are familiar with IIS, the Microsoft .Net Framework 
To develop for BowerBird, you will need to ensure the following development machine configuration:
* Microsoft Windows 64 bit Environment running .Net Framework version 4
* Visual Studio 2010 installed with Asp.Net MVC3 for web based development (or the solution file will not load properly). If you need to install Asp.Net MVC, use the downloadable installer - the Web Platform Installer takes a very long time.
* Node.js installed and added to the system PATH variable
* MSBuildTasks - Download and install the MSI of this project: https://github.com/loresoft/msbuildtasks from this link: http://code.google.com/p/msbuildtasks/downloads/list
* Use git (recommended) to download the source or pull it down as a zip file to your environment


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


First Compilation:
==================
Once you have the source code downloaded and in place, open up the solution file Bowerbird.sln in the root folder. 
* Make the output window visible (Debug > Windows > Output). 
* Build the solution. 
* Nuget will download all the dependent third party assemblies and run a build. THIS BUILD WILL FAIL. 
* Now that you have all the required assemblies, you need to run the build script by opening a console and navigating to and running [path to root]\Build\Build.bat.
* This will create the main-combined.js file that the environment depends on to locally build within Visual Studio for development. It's a bit of a chicken or egg situation. 


Setting up RavenDB on your Environment:
=======================================
This part is unusual if you are new to RavenDB but as you will see, it's very easy. We will set up an in-memory ravenDB instance. This particular configuration is great for development. You might also look into running RavenDB in memory, but with local storage - which means every time you istart your RavenDB session, it already has data. When it comes to hosting, you would instead host RavenDB in IIS or run it as a service.
* Go to the [source code folder]\packages\Ravendb.version.server\
* Because we are running with an older, stable version of RavenDB for production, we can't pull the server source from NuGet. You need to go and download build 960 from http://hibernatingrhinos.com/builds/ravendb-stable-v1.0/960. Extract this zip to a local folder. This is where it gets magical.
* Navigate to the [Raven Source code]\Server directory. Right click on the Raven.Server application icon. Pin it to your Task Bar. Once the Maroon Raven icon appears on your task bar, Right click on it, Right click on the Raven icon in the context popup and select properties. In the top text field in the 'General' tab, give the shortcut a name, like 'Raven DB 960'. On the 'Shortcut' tab, in the 'Target' field, at the end of the shortcut path (...Server\Raven.Server.exe), add the switch -RAM so that it will now read like this:  ..Server\Raven.Server.exe -RAM
* Ok out of this dialog then click on the RavenDB shortcut on your task bar. Raven will initialize after a few moments in console in RAM. It will display the server url. Paste this URL into your web browser. You will need to have silverlight installed. If not it will prompt you to install. otherwise, welcome to your raven database.
* To run the website, you will need to have this console running, and you will need to set up the ravendb url as your documentStore setting in the web.config.


Web.Config
==========
You will need to edit the web.config file where it is commented with: <!--YOU WILL NEED TO CHANGE THESE SETTINGS-->
* documentStore: databaseName refers to the RavenDB Tenant. If single tenant (like running in ram), leave blank. url is the path to the ravenDB instance. RavenDB in itself is a big learning curve. Checkout the docs and jabbR user groups as referenced on the www.ravendb.net site
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


First Website Run
=================
Bowerbird has over 200,000 species that need to be loaded and indexed. This will happen on the first load as part of the system setup. This may take a long time
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
These instructions have been tested on a blank slate Dell laptop circa 2008 model and have worked. Make sure you read and follow them carefully and they should work for you too.
If you have any issues or feature requests or improvements for this document, submit them to https://github.com/Bowerbird/bowerbird-web/issues


Have Fun!
=========
Frank - frank@radocaj.com
Hamish - hamish.crittenden@gmail.com
Ken - kwalker@museum.vic.gov.au