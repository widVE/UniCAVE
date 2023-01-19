---
layout: post
title: UniCAVE and HLAPI Support
---

Hey all, we're still here.  For those running into problems related to HLAPI networking errors, this is due to that package being deprecated in Unity 2021.  We are slowly working on an update to support Unity's new networking system, but in the meantime, the HLAPI can still be used by manually adjusting your package.json and manifest.json files for your project.  For example, they should include the following:

manifest.json - add the following line:

  "com.unity.multiplayer-hlapi": "1.1.1",

packages-lock.json add the related lines:

    "com.unity.multiplayer-hlapi": {
      "version": "1.1.1",
      "depth": 0,
      "source": "registry",
      "dependencies": {
        "com.unity.nuget.mono-cecil": "1.10.1"
      },

This should allow these packages to import still and thus fix any networking related errors.