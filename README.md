#### Where can I download it?
**[Click here to download v1.2 for Windows](https://github.com/inlinevoid/HLDJConverter/releases/download/1.2/HLDJC.1.2.Full.zip)**

#### What does it do?
Converts your music/media into a format that HLDJ can play.

#### How do I use it?
1. Run `HLDJConverter.exe`
2. Set the `Output Folder` to the directory you want converted songs saved to.  Usually `HLDJ\Audio`.
3. Drag any media files (.mp3, .ogg, .mp4, .wav, etc.) or Youtube links into the drop area.
4. That's it!

#### What media formats can it convert?
HLDJConverter uses [FFmpeg](https://www.ffmpeg.org/general.html#Audio-Codecs) to do all of the conversions therefore anything FFmpeg can convert, HLDJ can convert.

#### Hold on, did you say I can convert "Youtube links"?
Yes! Dragging a youtube link into HLDJConverter will automatically download the video in the highest quality possible and convert it.

#### It's not working and I'm getting errors!
Alright!  No problem, there's a few things you can do:
* Submit an issue here on Github.
* Send me an email (inlinevoidmain@gmail.com)
* Fix it yourself and submit a pull request.

It's very likely that Youtube functionality will break as time goes on due to the Youtube download process constantly changing.  So if it stops working for you, it's probably broken and needs to be updated.  Let me know!

#### Screenshots
![Screenshot](http://i.imgur.com/h77YRC9.png)

#### Changelog
###### v1.2
- Added a null check for the output folder.
- Fixed a crash when trying to convert Firefox .url files.
- Replaced FFmpeg.exe with the 32-bit version

###### v1.1
- Added more descriptive error messages for failed Youtube downloads.
- Added support for dropping .url files and parsing out the Youtube links.
