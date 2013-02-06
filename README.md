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

To develop for BowerBird, you will need to ensure the following development machine configuration:
* Microsoft Windows 64 bit Environment
* Visual Studio 2010 installed with MVC3 for web based development
* Node.js installed and added to the system PATH variable
* MSBuildTasks - Download and install the MSI from this project: https://github.com/loresoft/msbuildtasks


Running Locally
===============

To build and run the source code for BowerBird on your development machine you will need to ensure:
* You have the NUGet Package Manager installed: http://nuget.org/
* You have an instance of RavenDB you can point your BowerBird instance to


Build Script
============
There is a build script at /Build/build.bat
* For command switch arguments and directions on use, read /Build/getting-build-to-work.txt
* If this build script runs successfully, you are in a state to develop and deploy your own version of BowerBird
* If you have any trouble, submit a bug to https://github.com/Bowerbird/bowerbird-web/issues