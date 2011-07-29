# README #
## Licensing ##

Copyright 2010 Frank-Leonardo Quednau ([realfiction.net](http://realfiction.net)) 
Licensed under the Apache License, Version 2.0 (the "License"); 
you may not use this solution except in compliance with the License. 
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, 
software distributed under the License is distributed on an "AS IS" 
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 

## 1.1.2 ##

Fixed bug related to removing and adding subscription where under certain conditions
the subscription will not be picked up for handling a message

## 1.1.0 ##

PublishToken now has a Cancel property. 
If a publish pipeline member sets this to true, all subsequent pipeline members will not be called anymore

## 1.0.4 ##

Added Infrastructure class "DeferredPublishPipelineMember" to separate the time-wise coupling of setting up the bus and being able
to construct an instance of "IPublishpipelineMember"

## 1.0.3 ##

Removal of classes from MemBus that have no direct relationship to the core function of MemBus.
