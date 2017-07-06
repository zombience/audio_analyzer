# README #

AudioAnalyzer is intended to be a simple tool for getting 4-band amplitude from an audio stream in Unity3D, via either real-time (microphone, line in) or baked audio, and driving behaviors based on the amplitude data. 

There are four basic scripts that use this amplitude information to drive behaviors. These scripts provide basic examples of what is possible, and also provide a framework which can be extended to build your own fx. 

### DISCLAIMERS ###
This package has only been tested on PC / OSX, and is not guaranteed to work on any other platform. If you have success on mobile, I'd love to hear about your experience. 

If you are using realtime input, there is currently a gap when the realtime buffer is filled as the buffers are swapped, and will be perceived as a pop or a gap in audio, depending on the speed of your system. I have not intended this package to be used for playithrough of realtime input. I will address this issue in a future update.


## QUICKSTART ##

Open
Assets > Scenes > tests > audioTest.unity 

Press play, and you will see four cubes move and change color based on the frequencies analyzed in the provided sample audio.

![reactive scene](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/reacting_cubes.PNG)

To get started quickly, simply duplicate the scene and manipulate objects as you'd like, change settings and add new objects  

## AudioAnalyzer Inspector ##

### Requirements ###
![AudioAnalyzer Inspector](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/audioanalyzer_inspector.PNG)

For AudioAnalyzer.cs to work properly, it must: 
* have an Audio Source component attached to its game object
* have an output group assigned to the Audio Source

![audiosource mixer property](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/audiosource_mixer.PNG)
* have the volume parameter of that mixer group be exposed

![mixer](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/audio_mixer.PNG)
* the volume parameter must be named "InputVolume" (case sensitive)
* have the mixer containing the above output group assigned to the "Mixer" parameter on the AudioAnalyzer component

![audio analyzer mixer property](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/audioanalyzer_mixer.PNG)
* have either an input audio stream available (line in or mic), or have an audio file to read from

### Explanation of controls ###

![AudioAnalyzer Inspector](https://raw.github.com/zombience/audio_analyzer/git_images/images_for_github/audioanalyzer_inspector.PNG)

#### Crossovers ####
these are the crossover points between audio bands, so these numbers essentially control what constitutes "low", "mids", and "highs" etc. 
DISCLAIMER: because I am using a reduced sample size, these are more or less "magic numbers" that were derived by playing with settings until fx were reacting in what seemed like an appropriate manner. You may want to change them based on the type of source audio you're using. Your mileage may vary. 

In a future update I will have a variable number of audio bands, but currently analysis is limited to 4 bands. 

#### Band Gain ####
this is the amount to boost the gain of each band. 
increasingly high frequencies in the audio spectrum tend to have less energy (i.e. lower amplitude) and must be boosted to acheive an adequate effect in this context. 
bands may also need to be boosted / attenuated based on different source audio. 
the saved settings are, again, "magic numbers" which have been derived by trial and error.

#### Ease Ampitude ####
this setting determines whether the amplitude provided by Audio Analyzer should transition smoothly from preious values to current values, or whether changes should happen instantaneously. 
Instantaneous changes are a bit more frenetic and generally result in more jagged behaiors, whereas eased values achieve a more classic, expected behavior. 

#### Rise / Fall rate ####
this is the speed at which values should update to the newest values. lower values cause slower and smoother updates, at the expense of some accuracy.  

#### Listen ####
this is included as a debug-feature only. it allows for playing the analyzed audio through unity's chosen output (speakers, headphones, etc.) 

DISCALIMER: 
if you are using realtime input, there is currently a gap when the realtime buffer is filled as the buffers are swapped, and will be perceived as a pop or a gap in audio, depending on the speed of your system. 

~~i will address this issue in a future update.~~
I am planning an update that will address this issue, but it will be a major API-breaking update. 
Several issues will be addressed, and a new audio-analysis core will be implemented along with various other improvements. 

#### Mixer ####
The main mixer that contains the output group that the attached Audio Source has assigned. 
#### Clip ####
the chosen baked audio file to be used in case of non-realtime input

#### Input Selection Key ####
defaults to "I", this is the key to press during play to switch audio input methods (e.g. microphone, line in, etc)


## Future Developments and Plans ##

* i plan to create a polymorphic structure for fx, instituting a base class and interface for ease of extensions and custom fx
* i will implementing windowing on bands, so that audio data is more realistic / accurate
* variable number of bands will be implemented for more or less detail

