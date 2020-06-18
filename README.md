
# UniCAVE Plugin version 2.0
Ross Tredinnick, Brady Boettcher, Sam Solovy, Simon Smith, Benny Wysong-Grass, Kevin Ponto
2/4/2019

A Unity3D Plugin for Non-Head Mounted Virtual Reality Display Systems

The UniCAVE Plugin is a solution for running Unity within immersive projection VR display systems.

# Setup and Documentation

For installation info, see the [Web Documentation for 2018 / 2019 Installation](https://unicave.discovery.wisc.edu/2018-documentation/).

For detailed script info, see the [Web Documentation for 2018 / 2019 Scripts](https://unicave.discovery.wisc.edu/unicave-2018-scripts-documentation/).

For a basic example, see the [Web Documentation for 2018 / 2019 Basic Custom Setup](https://unicave.discovery.wisc.edu/unicave-2018-scripts-documentation/#BasicCustomSetup), or look at the sample prefabs in each Unity project (UniCAVE2017, UniCAVE2018, UniCAVE2019).

# Status and Update History

For Unity 2018.X and Unity 2019.X, the UniCAVE 2018 and UniCAVE 2019 projects represent the up-to-date version 2.0 of UniCAVE. A legacy version of UniCAVE, versions 1.X, is available for Unity 2017.X and 5.X, respectively. Documentation is available for all version of UniCAVE at https://widve.github.io/UniCAVE/wiki.

We are currently taking requests for developing custom prefabs meant to work with your immersive projection setup, or feel free to try yourself by taking a look at the documentation on the website.

**Important: UniCAVE uses the Unity Multiplayer HLAPI for local networking. If your project does not use this, it needs to be added to your Unity project in the Unity Package Manager (Window -> Package Manager)**

1/28/2020 - Minor changes listed below:
* HeadConfiguration can now instantiate prefabs with camera script, instead of just plain cameras
* Source commented

1/13/2020 - Changes listed below:
* Thanks to Christoffer A Tr?en (github freshfish70) for major contributions to the changes below:
* Improved warp calibration functionality from 2x2 quad to 8x8 mesh:
  * Warp can be loaded at runtime or be baked into the prefab
  * Post process layer can be assigned per display (default = 1 << 10)
* Improved realtime calibration functionality as well:
  * \+ or -: Increase or decrease brush size for moving adjacent vertices
  * WASD: chanes the selected vertex in an 8x8 grid
  * Arrowkeys: moves the selected vertex
  * Enter: advances to next display
  * Home: assigns internal vertices based on corners (WARNING: Overwrites internal vertex data)
  * Visual indicator for which vertex is being modified

12/2/2019 - Minor changes listed below:
* PhysicalDisplayManager added resolution field which affects script generation

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
