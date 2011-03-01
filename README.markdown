Stact - An actor library and framework for .NET
=======

# LICENSE
Apache 2.0 - see docs/legal (just LEGAL in the zip folder)

# IMPORTANT
NOTE: If you are looking at the source - please run build.bat before opening the solution. It creates the SolutionVersion.cs file that is necessary for a successful build.

# INFO
## Overview
Developing concurrent applications requires an approach that departs from current software development methods, an approach that emphasizes concurrency and communication between autonomous system components. The actor model defines a system of software components called actors that interact with each other by exchanging messages (instead of calling methods on interfaces in an object-oriented design), producing a system in which data (instead of control) flows through components to meet the functional requirements of the system.

Stact is a library for building applications using the actor model in .NET. The main assembly, Stact.dll, is the actor library and includes everything needed to use the actor model in any type of application. There are also additional supporting frameworks, such as Stact.ServerFramework, that can be used to expose actors via sockets or HTTP, allowing services to be built using actors.

Stact has a number of using features, including:

* Actors, both typed and anonymous
* Fibers, a cooperative threading model
* Channels, to support message passing between objects
* Workflow, allowing complex state-driven protocols to be defined and executed
  
## Getting started with Stact
### Downloads
You can also download Stact from the build server at [http://teamcity.codebetter.com](http://teamcity.codebetter.com).
  
    
### Source
This is the best way to get to the bleeding edge of what we are doing.  

1. Clone the source down to your machine.  
  `git clone git://github.com/phatboyg/Stact.git`  
2. Type `cd Stact`  
3. Type `git config core.autocrlf false` to set line endings to auto convert for this repository  
4. Type `git status`. You should not see any files to change.
5. Run `build.bat`. NOTE: You must have git on the path (open a regular command line and type git).  
  
# REQUIREMENTS
* .NET Framework 3.5/4.0  
  
# RELEASE NOTES
  
# CREDITS

