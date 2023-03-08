using System.IO;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;
using Microsoft.Maui.Graphics;
#if WINDOWS
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.Graphics.Canvas;
#endif
#if IOS || ANDROID || MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#endif

namespace FoxitPDFViewCtrl.Drawables;

public class PDFViewDrawable : IDrawable
{
    private byte[]? data_ = null;
    private byte[]? lastdata_ = null;

    public void SetData(Stream stream)
    {
        MemoryStream memStream = new MemoryStream();
        stream.CopyTo(memStream);        
        data_= memStream.ToArray();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (data_ != null)
        {
            if(lastdata_!= null) 
            { 
                if(data_.Equals(lastdata_))
                {
                    return;
                }
            }

            lastdata_= new byte[data_.Length];
            Array.Copy(data_, lastdata_, data_.Length);
            IImage image;
            MemoryStream mem_stream = new MemoryStream(data_);
#if WINDOWS
            W2DImageLoadingService w2d = new W2DImageLoadingService();            
            image = w2d.FromStream(mem_stream);
#endif
#if IOS || ANDROID || MACCATALYST
            image = PlatformImage.FromStream(mem_stream);
#endif

            canvas.DrawImage(image, 0, 0, dirtyRect.Width, dirtyRect.Height);
        }
    }
}