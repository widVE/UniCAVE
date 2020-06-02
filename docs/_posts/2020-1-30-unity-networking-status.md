---
layout: post
title: Unity Networking Status
---

UniCAVE currently uses the “Multiplayer HLAPI” to perform local network synchronization of master and client machines. This API has been deprecated and is not included by default in new versions of Unity, and is due to be replaced with a different API. However, it will be available and supported in the package manager until at least early 2021.

As such, it is necessary to include the Multiplayer HLAPI package in UniCAVE projects; This can be done in the Unity Package Manager, found in Window -> Package Manager.

This information was clarified in [this blog post by Unity](https://support.unity3d.com/hc/en-us/articles/360001252086-UNet-Deprecation-FAQ).