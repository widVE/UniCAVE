# UniCAVE Plugin version 1.4
Ross Tredinnick, Brady Boettcher, Sam Solovy, Simon Smith, Benny Wysong-Grass, Kevin Ponto
2/4/2019

A Unity3D Plugin for Non-Head Mounted Virtual Reality Display Systems

# Contents

The Uni-CAVE Plugin is a solution for running Unity within immersive projection VR display systems. 
 
See the documentation in the root folder of the repository for installation of the plugin.

The plugin has been tested on Unity versions 5.4.0f3 and 5.5.0f3  

We are currently taking requests for developing custom prefabs meant to work with your immersive projection setup, or feel free to try yourself by taking a look at the examples within the "ExampleScenes" folder and following the guide in this documentation or on the website.

7/19/2019 - Minor changes listed below:
* PhysicalDisplayManager now logs viewport changes to child PhysicalDisplays
* PhysicalDisplayManager options now require fullscreen (this behavior was implicit in the past)
* PhysicalDisplay no longer has specific display option if it is managed
* PhysicalDisplay disables unneeded eyes on XR cameras (on left cam, right eye is disabled, etc)
* PhysicalDisplay settings import/export now easier to use
* Separated Windows window resizing functionality into a utility class to simplify PhysicalDisplay
* UCNetwork launch script generator places backslash in front of executable name

2/4/2019 - Support for Unity 2018 has been added.  Note: testing has occurred only on Unity 2018.3.  There have been other streamlining of the process of setting up prefabs, amongst other improvements.  See the documentation on the unicave.discovery.wisc.edu website.

9/30/2017 - We've added separate Unity 2017 and Unity 5.5 versions.  Also, a great deal of UI and other features and enhancements have been added by way of contributions made by the Idaho National Laboratory!

8/9/2017 - We have added the Kinect integration assets and code that appeared to be missing.  Work is also in progress on Unity 2017 support.

5/17/2017 - We have separated out the example scenes into a separate repository : https://github.com/livingenvironmentslab/UniCAVE_Examples to reduce the size of the repository when downloading.
