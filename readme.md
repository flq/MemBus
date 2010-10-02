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

## Dependencies ##

Dependencies can be found in the "**lib**" subfolder. MemBus currently has no mandatory dependencies. 
If you want to use the Service Location feature for resolving e.g. handlers, also deploy the **Microsoft.Practices.ServiceLocation.dll** 
(taken from [codeplex](http://commonservicelocator.codeplex.com/)).

To have more fun with the Observables, it is recommended you use the Reactive framework. The 2 important dlls are **System.Reactive.dll** and **System.CoreEx.dll**.

The additional WPF Application dependencies are contained in the **WpfApp subfolder** of the lib:

* [Caliburn.Micro.dll](http://caliburnmicro.codeplex.com/)
    * System.Windows.Interactivity.dll
* [Twitterizer2.dll](http://www.twitterizer.net/)
    * Twitterizer.Data.dll
    * [Newtonsoft.Json.dll](http://james.newtonking.com/pages/json-net.aspx)
* [StructureMap](http://structuremap.sourceforge.net/)