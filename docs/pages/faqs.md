---
layout: page
title: FAQ's
---

Have a question not answered here? Feel free to [contact us](contact.html).

### Stereo and Windows 7

It’s come to our attention that the Virtual Reality Supported stereoscopic non-head mounted option for a DirectX build doesn’t work in Windows versions prior to 8.1 Users running Windows 7 should turn to an OpenGL build using the glIntercept dll stereo plugin.
Further information: [docs.unity3d.com/Manual/XR.html](https://docs.unity3d.com/Manual/XR.html)


### MSAA in Unity 2017

Some users have reported problems with trying the plugin on Unity 2017. It appears that Unity 2017 is defaulting MSAA on for all cameras, which may be conflicting with setting the virtual reality supported – stereo (non-head mounted) option. The authors are beginning work on an updated version of the plugin for 2017, in the meantime, if your setup uses the VR supported stereo non-head mounted, be sure to uncheck Allow MSAA on all cameras that also use stereo.


### I seem to have everything setup but I am seeing a black screen, or nothing drawing

Here are some things to check:

Machine name variables on scripts are case sensitive.  Make sure the names and variable names match up directly.
The MasterTrackingData’s Multiple Display checkbox should be checked if you are using several displays with a DirectX build.
If you still have issues feel free to [contact us](contact.html).


### Do I need a certain version of Unity3D to use this plugin?

This plugin should work on any version of Unity 5.4+.


### My system has multiple displays but the application only launches on a single display even though I checked the multiple display box. Why is this happening?

It is very likely that Unity is launching in full-screen mode, something that can not span across multiple displays. Use the “–screen-fullscreen 0” as a command line argument to force the system to not use fullscreen mode.


### I’ve set up the project as specified, but it won’t run on my system? How do I debug this?

Two common issues with the plugin are syncing nodes and establishing tracking. If the application is failing to launch entirely or crashing on startup, be sure to check the log files to see if there are any obvious errors. Also – the VRPNTrack.cs and VRPNInput.cs scripts have optional Debug Output check boxes that one can enable to see additional output in the log files.


### I see strange behavior when using MSAA and HDR

This is due to that fact that Unity’s Forward rendering path does not support MSAA when using HDR render buffers. For more information follow this post here


### How can I make contributions to this project or add my custom prefab to the existing prefab base?

If you’ve successfully created a prefab for your system and would like to share it with other users, consider making a push request on our Github by putting your new prefab in the Assets/Prefabs folder.


### Who do I contact if I have questions/comments about the plugin?

You can contact unicave@lists.wisc.edu with any questions/comments about this plugin. You can also start a discussion on the Github page.