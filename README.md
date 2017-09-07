# README #

## !BETA! ##
Welcome to the beta branch of AudioAnalyzer!
Thank you for checking this out. Please message me with any feedback and desired features. Feel free to open issues if you find bugs. As for feature requests, I'll consider them, although I may or may not have time to get to them. 
Happy analyzing!



AudioAnalyzer is intended to be a simple tool for getting 4-band (or less) amplitude from an audio stream in Unity3D, via either real-time (microphone, line in) or baked audio, and driving behaviors based on the amplitude data. 

This package is intended to be a basic framework to provide a starting point for you to develop your own behaviors. This is not intended to be a fully featured drag-and-drop performance tool. I've attempted to cover common needs and will continue adding features, but overall this is meant to help you get started implementing your own fun tools. 


### DISCLAIMERS ###
This package has only been tested on PC / OSX, and is not guaranteed to work on any other platform. If you have success on mobile, I'd love to hear about your experience. 


This Readme is for BETA!

## QUICKSTART ##

Open up the project, navigate to the "Scenes" directory to get started looking at examples. 



## IMPROVEMENTS ##

This release has lots of updates:

* AudioAnalyzer has new math and a new UI, mostly taken from ![Keijiro Takahashi's project](https://github.com/keijiro/Reaktion) Reaktion
** Includes much more accurate math to obtain aplitude for a given band
** Includes variable width ("Q") per band
* Updated inspectors for Transform AFX
* Material Property AFX, for modifying shader properties on a material (with auto-discovery and assignment)
* Drive animation controller properties
* lots of cleanup, organization, tweaks


## BIG THANKS ##

To Keijiro Takahashi, who put together the fine ![Reaktion](https://github.com/keijiro/Reaktion) package which the core analysis in this package is based on.
I've improved on it slightly by implementing multi-band analysis, whereas the Reaktion package is only overall amplitude. 

His package is more developed and feature-rich (including OSC and Midi in, which I may implement in the future), and has some pretty neat tools. 
I've attempted to keep this one fairly basic without too much framework cruft so that you can easily graft it into your own project and expand on it. 

And, because I used his code, which comes with a License, I am also required to post this License:

# License #

Copyright (C) 2013-2015 Keijiro Takahashi
Copyright (C) 2017 Jason Araujo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


