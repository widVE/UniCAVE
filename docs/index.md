## Description

The UniCAVE project aims to build a plugin for [Unity3D](https://unity3d.com) that provides support for CAVE environments utilizing features built into the engine. This approach enables a user to configure their system entirely in the Unity3D editor providing a simplified method for supporting adapting existing Unity projects for distributed visualization platforms.

The concept for the plugin is to have a three step approach for integration:

![Step 1](images\step1-768x133.png "Step 1")

![Step 2](images\step2-768x130.png "Step 2")

![Step 3](images\step3-768x131.png "Step 3")

This project is currently **_under development and is a work in progress_**.  Please feel free to _contribute to this project_ or read over the _[documentation]_(https://github.com/widVE/UniCAVE/wiki) to learn if the plugin is right for you.


### Current Features

- Native Unity3D time synchronization via Unity’s timeScale feature to distributed synchronization

- NetworkView RPC calls for synchronizing random seeds and dynamic objects

- VRPN Unity plugin to handle tracking systems

- In-editor preview of asymmetric view frustums