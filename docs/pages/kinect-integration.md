---
layout: page
title: Kinect Integration
---

We have added experimental support for the Microsoft Kinect with our plugin. In order to integrate the Kinect with your system, first install the Kinect SDK and developer toolkit (in that order) onto your head node machine from these links:

1. [http://www.microsoft.com/en-us/download/details.aspx?id=36996](http://www.microsoft.com/en-us/download/details.aspx?id=36996)
2. [https://www.microsoft.com/en-us/download/details.aspx?id=40276](https://www.microsoft.com/en-us/download/details.aspx?id=40276)

Our Unity package we include with our plugin contains the necessary Kinect scripts and prefabs, so there is no need to import the full Kinect package.

There are two objects required in the scene for the Kinect that are located in the **UniCave->Kinect** folder:

- kinectSkeleton (or other skinned mesh renderer)
- kinectPrefab

Drag both of these into your scene, and set the SW field of the kinectSkeleton equal to the kinectPrefab you just dragged in.

At this point, it’s best to mention how this works. We have added NetworkView components to each joint of the kinectSkeleton prefab so that the slave nodes on your system will all have the same transforms for the skeleton. If you’re using your own skinned mesh renderer, you’ll have to do the same. This is done so that the slave nodes do not have to install the Kinect SDK.