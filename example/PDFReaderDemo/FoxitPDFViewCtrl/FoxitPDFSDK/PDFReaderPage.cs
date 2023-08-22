using foxit.addon.xfa;
using foxit.common.fxcrt;
using foxit.common;
using foxit.pdf;
using foxit.pdf.interform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoxitPDFViewCtrl.FoxitPDFSDK
{

    public class PageParse_Pause : PauseCallback
    {
        public PageParse_Pause(int pause_count_limit = 0, bool always_pause = false)
        {

        }

        public override bool NeedToPauseNow()
        {
            return true;
        }
    }

    public class PDFReaderPage : IDisposable
    {
        private const int default_page_space_ = 6;
        private int index_;

        private bool has_transparency_;
        private float height_;
        private float width_;
        private float total_height_;
        private PDFPage? pdf_page_;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            Release();
        }

        void Release()
        {
            if (pdf_page_ != null)
            {
                pdf_page_.Dispose();
                pdf_page_ = null;
            }
        }

        public PDFPage GetPDFPage()
        {
            return pdf_page_;
        }

        public void SetPDFPage(PDFPage pdf_page)
        {
            if (pdf_page_ != null) pdf_page_.Dispose();
            pdf_page_ = pdf_page;
        }

        public bool ParsePage(bool is_reparse = false)
        {
            if (pdf_page_ == null || pdf_page_.IsEmpty())
                return false;
            if (pdf_page_.IsParsed() && !is_reparse)
                return true;

            // SDKRD-12103: In maui dotnet7 for iOS, using PageParse_Pause results in a crash (possibly due to bugs within maui itself). Therefore, it is advisable to avoid using PageParse_Pause at this time.
            //PageParse_Pause pause = new PageParse_Pause();
            Progressive progress = pdf_page_.StartParse((int)PDFPage.ParseFlags.e_ParsePageNormal, null, is_reparse);
            Progressive.State state = Progressive.State.e_ToBeContinued;
            while (state == Progressive.State.e_ToBeContinued)
            {
                state = progress.Continue();
            }
            has_transparency_ = pdf_page_.HasTransparency();
            return true;
        }

        public int GetPageIndex()
        {
            return index_;
        }

        public void SetPageIndex(int index)
        {
            index_ = index;
        }

        public void SetPageHeight(float height)
        {
            height_ = height;
        }

        public void SetPageWidth(float width)
        {
            width_ = width;
        }

        public void SetPageTotalHeightWithOutSpace(float total_height)
        {
            total_height_ = total_height;
        }

        public float GetPageTotalHeightWithSpace(float zoom)
        {
            return total_height_ * zoom + (index_ + 1) * default_page_space_;
        }

        public float GetPageHeight()
        {
            return height_;
        }

        public float GetPageWidth()
        {
            return width_;
        }

        public Stream GetRenderPageBuffer(PDFReaderDoc reader_doc, int bitmap_width, int bitmap_height, int cx, int cy, int width, int height, foxit.common.Rotation rotate)
        {
            try
            {
                ParsePage();
                // Prepare a bitmap for rendering.
                foxit.common.Bitmap bitmap = new foxit.common.Bitmap(bitmap_width, bitmap_height, foxit.common.Bitmap.DIBFormat.e_DIBArgb);
                foxit.common.Renderer render = new foxit.common.Renderer(bitmap, false);
                foxit.common.fxcrt.Matrix2D matrix = pdf_page_.GetDisplayMatrix(cx, cy, width, height, rotate);
                bitmap.FillRect(0xFFFFFFFF, null);
                // Render page
                render.StartRender(pdf_page_, matrix, null);
                int pitch = bitmap.GetPitch();
                IntPtr ptr = bitmap.GetBuffer();

                SKImage sk_image = SKImage.FromPixels(new SKImageInfo(bitmap_width, bitmap_height, SKColorType.Bgra8888), ptr, pitch);
                var data = sk_image.Encode();
                Stream stream = data.AsStream();

                render.Dispose();
                bitmap.Dispose();
                return stream;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
