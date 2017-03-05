# UniCAVE Plugin version 1.0
Ross Tredinnick, Brady Boettcher, Sam Solovy, Kevin Ponto, Simon Smith
3/4/2017

A Unity3D Plugin for Non-Head Mounted Virtual Reality Display Systems


Contents
The Uni-CAVE Plugin is a solution for running Unity within immersive projection VR display systems. 
 
The following document will guide on setup of the plugin within Unity.  The plugin has been tested on Unity versions 5.4.0f3 and 5.5.0f3  

We are currently taking requests for developing custom prefabs meant to work with your immersive projection setup, or feel free to try yourself by taking a look at the examples within the "ExampleScenes" folder and following the guide in this documentation or on the website.

To install and setup the UniCAVE plugin, perform the following steps:

Download and install one of the above versions of Unity.

Pull the plugin or download a zip of the UniCAVE plugin and extract it to a location of your choice.

Open Unity and start a new project.

Click the “Assets” menu within the Unity editor, then click the “Import Package” sub-menu and choose Custom Package.

Browse to and select CavePlugin.unitypackage.

Upon importing the package, several assets will appear in your project within the Scripts, Plugins and Prefabs directories.  Drag the <Your Lab>_Holder.prefab file into the scene.

Various global settings for the scene can be set on the MasterTrackingData.cs script.  These values include eye offsets, whether multiple displays need to be activated (set this to true if you need more than one viewport for rendering to your immersive system), tracking system offsets, default background clearing for all of the cameras, near and far clip values, etc. To sync any changes here with all of the left/right eye cameras, you can drag the script onto the “CameraHolder” object, then right click on the Inspector entry “Master Tracking Data (script)” and choose “Sync Camera Settings”.

Next you need to setup some tracking system related information.  Click the “planeHolder” object within the prefab you imported and find the “Tracker Host Settings” component.  Under hostname – enter the name of the PC connected to your tracking system.  For type – keep as standard for VRPN usage – there is also a “Vicon” option but this is currently un-tested.

Next click the “Camera Holder” object.  For the Cave Tracker Settings, under Object Name – enter the head tracker address for connecting to your tracking system and the channel you wish to track.  I.e. for our CAVE this is “Isense900” and channel 0.  

Perform the same as in the preceeding paragraph for the “TrackerRotation”, “Wand” and “WandControls” objects.  (Note you may have a different address for the Wand – and different channels, i.e. ours is “Wand0” and channel 1.

Next click the children objects within “Camera Holder”.  Each has a ProjectionPlane.cs script.   For each object, find this script and for the Machine Name entry, enter the name of the machine for which your projector / TV / etc. is attached to.  (For us we have one machine per wall – this may be different depending on your system setup – perhaps you have the same machine for all walls, go ahead and enter that same machine name for each object).

Next some adjustments need to be made within the VRPN.cs script inside of the LELUnityPlugins\Scripts\VRPN Scripts folder.  The two functions vrpnTrackerPos and vrpnTrackerRot should be adjusted to reflect the transformation from your tracking system to Unity’s world space (z in/out, y up/down, x left/right).  Any sort of tracking system offset from your CAVE can be done in the vrpnTrackerPos function while any rotation can be done in vrpnTrackerRot.  Our system’s default values may work, but probably not.

Finally, there is a networkingSync.cs script attached to the Camera Holder object (if not, try adding).  Insert values on this script where hostname matches the IP address of your head node, port is a free port on the local network, and Head Node is the machine name of the machine your tracking system is connected to.

Once these adjustments have been made, go ahead and build your project.  Choose x86_64 and a Windows build (only tested on Windows as of 10/18 – will be tested on Linux soon).  

Here is a screen shots of the necessary Player Settings.  The plugin is meant to be run with OpenGLCore as the only Graphics API.
![Alt text](https://unicave.discovery.wisc.edu/wp-content/uploads/2017/02/settings1.png "Settings1")
![Alt text](https://unicave.discovery.wisc.edu/wp-content/uploads/2017/02/settings2.png "Settings2")
 	 
Now, wherever you’ve built your exe, take the packaged “GLQBStereo.dll” and “OpenGL32.dll” files and place them in the same directory as your exe.  In addition, open the “config.ini” file and change the “NumViewports” option to match the number of stereo viewports you have in your scene per machine.  I.e. if you run 4 stereo walls on a single machine, set this value to 8.  Finally in the packaged “gliConfig.ini” file, scroll all the way to the bottom to the “PluginData” section.  Change the BaseDir to the directory of your exe.  Copy the ini files plus the “GLFunctions” directory to the location of the exe as well.
Run the exe using whichever cluster-based launcher tool you use to launch your VR applications!  When running the exe, supply a –popupWindow command line argument.

Todo List
Currently input buttons from a tracked wand device are weakly supported – this will be improving in coming weeks, in addition to support for a game pad.
