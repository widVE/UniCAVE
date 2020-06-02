---
layout: post
title: Stereo Clarifications
---

Those of you trying to work with DirectX stereo via the “Stereo Display non-head mounted” option, see requirements within the Unity documentation at this link:

[https://docs.unity3d.com/Manual/StereoscopicRendering.html](https://docs.unity3d.com/Manual/StereoscopicRendering.html)

Also – I have confirmed an issue that I have heard of from some users regarding multiple displays and the above mentioned stereo option – here is an official issue within Unity’s issue tracker regarding it:

[https://issuetracker.unity3d.com/issues/stereo-display-vr-does-not-render-on-multiple-displays](https://issuetracker.unity3d.com/issues/stereo-display-vr-does-not-render-on-multiple-displays)

I’ve locally confirmed that this happens on a new power wall we’re setting up – potential work-arounds until this is resolved:

– Try an OpenGL build with DLL injection
– For those using Quadro cards – use nVidia’s Mosaic mode to create a single display
– Attempt to use a different stereo technique (Side by Side, Top / Bottom) if your hardware supports this.

Cheers!