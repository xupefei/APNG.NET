#APNG.NET
*A fully-managed APNG Parser*

----------
*If you don't know what is APNG, click [here][1].*

[1]: http://en.wikipedia.org/wiki/APNG

######Current version: 0.2

----------
##Introduction
I've been searching for days looking for an simple, easy-to-use animation controller for my game engine until I found [this article][2]. Then I noticed I can use APNG to bundle multiply image into one single file and describe the animation process internally (no coding needed). In APNG format, each frame have an `fcTL` chunk (frame control chunk), which contains many useful information such as `frame_height`; `x_offset` and `delay_time`. So we can set all these up when we build an APNG and copy it directly to game folder - no any coding needed.

[2]: http://www.codeproject.com/Articles/36179/APNG-Viewer

----------
##PNG and APNG specification support status

*   For simple PNG, All chunks but `IHDR`, `IDAT` and `IEND` are **unsupported**, and will be **ignored** during the parsing.
*   For APNG, the library **can only** parse `IHDR`, `acTL`, `fcTL`, `IDAT`, `fdAT` and `IEND` chunks.
*   Multiply frame size is **supported**. This means you can reduce the file size by using *Differential Frame*. (use [APNG Anime Maker][3])
*   All `DISPOSE_OP` and `BLEND_OP` is **supported**.

[3]: https://sites.google.com/site/cphktool/apng-anime-maker

----------

##What's next

*   Better error-handling
*   Better performance

----------
##About the code
This repository is made up by 3 parts:

*   **APNG Parser**
>   An managed DLL which handle parsing APNG image file.
>   This library is *backward-compatible*: It means you can use this library to read an simple PNG image, with no error produced.

*   **APNG Test Extractor**
>   An test application which can extract each frame of an APNG to an standalone PNG file.

*   **APNG Wrapper for [XNA][4]**
>   An simple game which use an APNG as animation.

[4]: http://en.wikipedia.org/wiki/Microsoft_XNA


*If you want to have a binary version of the code, [click here][0].*

[0]: https://github.com/xupefei/APNG.NET/downloads

To compile this project, you must have these components installed:

*   Visual Studio 2010
*   Microsoft XNA Game Studio 4.0 *or* Windows Phone SDK 7.1

----------
##Useful links

*   [PNG (Portable Network Graphics) Specification][5]
*   [APNG (Animated Portable Network Graphics) Specification][6]
*   [APNG Anime Maker][7]

[5]: http://www.libpng.org/pub/png/spec/1.2/png-1.2-pdg.html
[6]: https://wiki.mozilla.org/APNG_Specification
[7]: https://sites.google.com/site/cphktool/apng-anime-maker

----------
by [Amemiya][8], 2012.

[8]: https://plus.google.com/104849771033212826335