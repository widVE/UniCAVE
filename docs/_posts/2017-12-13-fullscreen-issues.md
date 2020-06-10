---
layout: post
title: Fullscreen Issues
---

A user sent this link out regarding possible fullscreen issues that can occur in Unity – this could be helpful for some who are running into related issues. It appears “Reseting” the Player Settings may be a solution – as well as unchecking a Force D3D Exclusive mode checkbox.

[https://answers.unity.com/questions/864434/unity-always-fullscreen-on-windows.html](https://answers.unity.com/questions/864434/unity-always-fullscreen-on-windows.html)

In addition, with the latest version of Unity 2017.2, our previous tile-based Linux display needed to have it’s -screen-fullscreen flag turned from off (0) to on (1), to correctly render top-bottom stereo on a 20 panel display system (in this case the top bottom displays had to also be swapped on the prefab, but this was seemingly due to Linux desktop layout).