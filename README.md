# Terri-Fried-MonoGame
This is a C# rewrite of the game [Terri-Fried](https://github.com/PolyMarsDev/Terri-Fried) by @PolyMarsDev based off of MonoGame rather than Raylib.

The DesktopGL platform used in this game "requires at least OpenGL 2.0 with the ARB_framebuffer_object extension (or alternatively at least OpenGL 3.0)". If you don't have those requirements, porting the code to other platforms such as WindowsDX or WindowsUniversal may work (it's easier than it sounds).

This game is available for Windows (Vista and up), macOS (High Sierra 10.13 and up) and Linux (64-bit only), and Android (armeabi-v7a) with builds available. This game can also be ported to other MonoGame-supported platforms, such as Android (done here) and iOS, with other target platforms.

# Enhanced
This branch aims to contain major improvements and enhancements over the accurate branch, increasing compatibility over target platforms and adding features the original Terri-Fried didn't have. Planned features are:

-  ̶a̶ ̶p̶a̶u̶s̶e̶ ̶m̶e̶n̶u̶ (added in v1.1)
- support for different framerates
- an options menu
- transitions
- different difficulties

# Differences
The main difference between the older version [here](https://github.com/Zolax9/Terri-Fried-MonoGame-Old) is the implementation of an 'Extra' class that contains any extra functionality not available in MonoGame to allow for minimal changes to the base game code. The Android build also has quite a bit of extra code to fit the game into the phone screen rather than an arbitrary 800x450 screen resolution.

# Screenshots
# Windows (DesktopGL)
![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot1.png) ![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot2.png)![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot3.png) ![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot4.png)
# Android
![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot5.png) ![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot6.png)![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot7.png) ![](https://github.com/Zolax9/Terri-Fried-MonoGame/blob/enhanced/screenshots/screenshot8.png)

# Known Issues
* The font sizes in MonoGame are slightly different from Raylib
