# Musical Pixels
 A visualiser of sorts for a 16-pixel Neopixel ring, with support for MP3

## What is it?
An audio visualiser utilising an Arduino with a ring of Adafruit Neopixels, with a desktop application sending data to the Arduino through USB Serial connection based on the current frequency and amplitude of the audio. Code for both is provided.

## Attributions / Libaries used
* My brother, [blujupiter32](https://github.com/blujupiter32), for some of the more complex formulas
* [ColorSpaceConverter](https://www.nuget.org/packages/ColorSpaceConverter/)
* [NAudio](https://www.nuget.org/packages/NAudio/)
* [GtkSharp](https://www.nuget.org/packages/GtkSharp/)

## How do I use it?
**All instructions performed with an Arduino Uno with a 16x ring of WS2812 Neopixels**

Make sure the Arduino is set up. At the moment, the data in slot is set to slot 6 on the Arduino.

Select an MP3 file and close the extra window that opens up (this is a consequence of making a last-minute inclusion of GTK into a Console application in an attempt to keep cross-platform compatibility)
![A screenshot of Windows Explorer selecting an MP3 file](https://imgur.com/O9qlblM.png)
The audio should begin playing out of your default audio device, and you should see the Neopixel array light up and change colour with the music. That's about it!
