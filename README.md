# Foxit PDF SDK for .NET MAUI

------

Foxit PDF SDK for .NET features a powerful, easy-to-use Core API in C# for rendering, viewing, annotation, signing, protecting and managing forms in PDFs. As a cross-platform product, Foxit.SDK.Dotnet 9.0 began to provide MAUI with interfaces supported by Windows, Android, iOS and macOS. The following sections introduce the contents of Foxit PDF SDK for .NET MAUI.

## Supported platforms

------

| Platform | System Requirements                                  | Memo                                                |
| -------- | ---------------------------------------------------- | --------------------------------------------------- |
| Windows  | Windows 11 and Windows 10 version 1809 or higher     | A PDFReader has been tested on Windows 10.0.19045   |
| MacOS    | macOS 10.15 or higher, using Mac Catalyst.           | A PDFReader has been tested on Mac OS 12.6.1        |
| iOS      | iOS 11 or higher, using the latest release of XCode. | A PDFReader has been tested on iOS 13.6.            |
| Android  | Android 5.0 (API 21) or higher.                      | A PDFReader has been tested on Android12.0 (API 31) |

## Getting Started ##

------

* [Install .NET MAUI](https://dot.net/maui) 
* [Install latest XCode for iOS and Mac Catalyst](https://developer.apple.com/xcode)
* [A sample PDFReader with Foxit PDF SDK for .NET MAUI](https://github.com/foxitsoftware/FoxitPDFSDKForMAUI)

## Using Foxit.SDK.Dotnet For MAUI  ##

------

### Install Foxit.SDK.Dotnet package

- Install Foxit.SDK.Dotnet package in Visual Studio using the NuGet Package Manager
- Install Foxit.SDK.Dotnet package in Visual Studio for Mac using the NuGet Package Manager
- Install and manage NuGet packages with the dotnet CLI
  `dotnet add package Foxit.SDK.Dotnet`
- Install Foxit.SDK.Dotnet NuGet packages with the NuGet CLI
  `NuGet\Install-Package Foxit.SDK.Dotnet`

### A  PDFReader Demo for .NET MAUI

#### A PDFReader demo for MAUI currently implements the following functions:

- PDF documents opening and closing.
- PDF pages rendering and displaying.
- PDF pages flipping and jumping.
- PDF pages zooming in and out.

#### Third-party nuget package needed

- **SkiaSharp**  Used to convert the rendered foxit.common.bitmap into an SkiaSharp.SKImage.
- **System.Drawing.Common**  Required by the Foxit.SDK.Dotnet.

#### Render a PDF page to Microsoft.Maui.Graphics.IImage

The following codes show how to convert foxit.common.bitmap to Microsoft.Maui.Graphics.IImage.

```C#
using IImage = Microsoft.Maui.Graphics.IImage;

...
foxit.common.Bitmap bitmap = new foxit.common.Bitmap(bitmap_width, bitmap_height, foxit.common.Bitmap.DIBFormat.e_DIBArgb);
foxit.common.Renderer render = new foxit.common.Renderer(bitmap, false);
foxit.common.fxcrt.Matrix2D matrix = pdf_page_.GetDisplayMatrix(cx, cy, width, height, rotate);
bitmap.FillRect(0xFFFFFFFF, null);
// Render page
render.StartRender(pdf_page_, matrix, null);
int pitch = bitmap.GetPitch();
IntPtr ptr = bitmap.GetBuffer();

SkiaSharp.SKImage sk_image = SkiaSharp.SKImage.FromPixels(new SkiaSharp.SKImageInfo(bitmap_width, bitmap_height, SkiaSharp.SKColorType.Bgra8888), ptr, pitch);
var data = sk_image.Encode();
Stream stream = data.AsStream();
MemoryStream mem_stream = new MemoryStream();
stream.CopyTo(mem_stream);        
IImage image;
#if WINDOWS
W2DImageLoadingService w2d = new W2DImageLoadingService();            
image = w2d.FromStream(mem_stream);
#endif
#if IOS || ANDROID || MACCATALYST
image = PlatformImage.FromStream(mem_stream);
#endif
...

```

For more information about Foxit PDF SDK for .NET MAUI (windows), please refer to Foxit PDF SDK for Windows dotnetcore API reference. For Foxit PDF SDK for .NET MAUI (iOS, Mac Catalyst and Android), there are no API references currently, but the interfaces are similar to Windows, you can refer to the Windows dotnetcore API reference first.