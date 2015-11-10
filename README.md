#### Where can I download it?
**[Click here to download v1.4 for Windows](https://github.com/inlinevoid/HLDJConverter/releases/download/1.4.1/HLDJConverter.1.4.1.zip)**

#### What does it do?
Converts your music/media into 22050hz mono channel, the current format for CSGO that [HLDJ](http://www.hldj.org/) can play.

#### How do I use it?
1. Extract the `HLDJConverter` folder from the zip file.
2. Run `HLDJConverter.exe`
2. Set the `Output Folder` to the directory you want converted songs saved to.
3. Copy/paste or drag/drop any files or Youtube links into the drop area.
4. That's it!

#### What media formats can it convert?
Some common formats include: `mp3`, `mp4`, `ogg`, `wav`, `flv`, `avi`, `mov`, `wmv`

HLDJConverter uses [FFmpeg](https://www.ffmpeg.org/general.html#Audio-Codecs) to do all of the conversions therefore anything FFmpeg can convert, HLDJ can convert.

#### Hold on, did you say I can convert "Youtube links"?
Yes! Dragging a youtube link into HLDJConverter will automatically download the video in the highest quality possible and convert it.

#### It's not working and I'm getting errors!
Alright!  No problem, there's a few things you can do:
* Submit an issue here on Github.
* Send me an email (inlinevoidmain@gmail.com)
* Fix it yourself and submit a pull request.

If HLDJConverter is crashing, a file called `HLDJConverterCrashLog.txt` should be created in the HLDJConverter directory.  At the top of that file is a link you can send me that will help debug the crash you're getting.

It's very likely that Youtube functionality will break as time goes on due to the Youtube download process constantly changing.  So if it stops working for you, it's probably broken and needs to be updated.  Let me know!

#### Screenshots
![Screenshot](http://i.imgur.com/uHlMNTG.png)

#### Changelog
###### v1.4
- Added an error message for invalid Youtube links
- Added crash logs
- Added automatic Gist upload of crash logs
- Added the ability to paste links/files using the `ctrl + v` shortcut
- Fixed a bug where converted files would silently overwrite files with the same name
- Added a check for invalid config file
- Fixed the crash when trying to convert folders
- Added `KeepWindowTopmost` option to the config file
- Minor UI changes
- Made dialog error messages more helpful

###### v1.3
- Added `OutputBitrate` option to the config file
- Added `OutputVolume` option to the config file

###### v1.2
- Added a null check for the output folder
- Fixed a crash when trying to convert Firefox .url files
- Replaced FFmpeg.exe with the 32-bit version

###### v1.1
- Added more descriptive error messages for failed Youtube downloads
- Added support for dropping .url files and parsing out the Youtube links
