using Microsoft.Maui.Graphics;
using System.Diagnostics;
using SkiaSharp;
using FoxitPDFViewCtrl.Drawables;
using FoxitPDFViewCtrl.FoxitPDFSDK;
using foxit;
using foxit.pdf;
using foxit.common.fxcrt;
using foxit.common;
using Microsoft.Maui.Controls;


namespace FoxitPDFViewCtrl.Controls;


public class PDFViewCtrl : ScrollView, IPDFViewCtrl
{
    private GraphicsView? graphics_view_ = null;
    private PDFViewDrawable? drawable_ = null;
    private StackLayout? layout_ = null;
    private PDFReaderDoc? current_doc_ = null;    
    private float zoom_factor_ = 1.0f;
    private foxit.common.Rotation page_rotation_ = 0;
    private float density_ = (float)DeviceDisplay.Current.MainDisplayInfo.Density; 

    public PDFViewCtrl()
    {
        graphics_view_ = new GraphicsView();
        drawable_ = new PDFViewDrawable();
        graphics_view_.Drawable = drawable_;
        layout_ = new StackLayout()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Content = layout_;
        layout_.Children.Add(graphics_view_);
#pragma warning disable CS0618 
        this.VerticalOptions = LayoutOptions.FillAndExpand;
        this.HorizontalOptions = LayoutOptions.FillAndExpand;
    }

    private void UpdatePageView(double width, double height, int page_index)
    {
        if (current_doc_ == null) { return; }
        current_doc_.UpdatePDFPage(width, height, foxit.common.Rotation.e_Rotation0, true);
        PDFReaderPage reader_page = current_doc_.GetPage(page_index);
        if (reader_page == null) { return; }
        float scale = zoom_factor_;
        foxit.common.Rotation rotate = page_rotation_;
        float r_width = 0, r_height = 0;
        if (rotate == foxit.common.Rotation.e_Rotation0 || rotate == foxit.common.Rotation.e_Rotation180)
        {
            r_height = reader_page.GetPageHeight() * scale;
            r_width = reader_page.GetPageWidth() * scale;
        }
        else
        {
            r_height = reader_page.GetPageWidth() * scale;
            r_width = reader_page.GetPageHeight() * scale;
        }
        RenderByGraphic(page_index, (int)r_width, (int)r_height, 0, 0);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (current_doc_ == null) { return; }
        UpdatePageView(width, height, current_doc_.GetCurrentPage());
    }

    
    private void RenderByGraphic(int index, int width, int height, int cx, int cy)
    {
        int bitmap_width = width;
        int bitmap_height = height;
        if (density_ > 1)
        {
            bitmap_width = (int)(width * density_);
            bitmap_height = (int)(height * density_);
        }
  
        foxit.common.Rotation rotate = page_rotation_;
        int render_left = cx < 0 ? cx : 0;
        int render_top = cy < 0 ? cy : 0;
        if (current_doc_ == null) { return; }
        foxit.common.Rotation render_rotation = rotate;
        PDFReaderPage reader_page = current_doc_.GetPage(index);
        if (reader_page == null) { return; }

        Rect page_rect = new Rect(cx, cy, bitmap_width, bitmap_height);

        Stream stream = reader_page.GetRenderPageBuffer(current_doc_, (int)page_rect.Width, (int)page_rect.Height, render_left, render_top, bitmap_width, bitmap_height, render_rotation);

        if (drawable_ != null && graphics_view_ != null && stream != null)
        {
            stream.Position = 0;
            drawable_.SetData(stream);
            graphics_view_.WidthRequest = width;
            graphics_view_.HeightRequest = height;
            graphics_view_.Invalidate();
        }
    }

#region opendoc/closedoc/savedoc
    public void SetDoc(PDFDoc doc)
    {
        if(doc != null && current_doc_ != null)
        {
            current_doc_.SetDoc(doc);
        }        
    }

    public PDFDoc? GetDoc()
    {
        if (current_doc_ != null)
        {
            return current_doc_.GetDoc();
        }
        else
        {
            return null;         
        }        
    }

    public void OpenDoc(string filepath, string password, Completion completion)
    {
        if (current_doc_ != null)
        {
            current_doc_.Dispose();
            current_doc_ = null;
        }
        current_doc_ = new PDFReaderDoc();
        current_doc_.SetView(this);
        foxit.common.ErrorCode error = current_doc_.OpenDoc(filepath, password);
        if (error == foxit.common.ErrorCode.e_ErrSuccess)
        {
            UpdatePageView(this.Width, this.Height, 0);
        }

        if (completion != null)
        {
            completion(error);
        }
    }

    public void OpenDocFromMemory(byte[] buffer, string password, Completion completion)
    {
        if (current_doc_ != null)
        {
            current_doc_.Dispose();
            current_doc_ = null;
        }
        current_doc_ = new PDFReaderDoc();
        current_doc_.SetView(this);
        foxit.common.ErrorCode error = current_doc_.OpenDocFromMemory(buffer, password);
        if (error == foxit.common.ErrorCode.e_ErrSuccess)
        {
            UpdatePageView(this.Width, this.Height, 0);
        }
        if (completion != null)
        {
            completion(error);
        }
    }

    public void OpenDocFromFileReader(FileReaderCallback file_reader, string password, Completion completion)
    {
        if (current_doc_ != null)
        {
            current_doc_.Dispose();
            current_doc_ = null;
        }
        current_doc_ = new PDFReaderDoc();
        current_doc_.SetView(this);
        foxit.common.ErrorCode error = current_doc_.OpenDocFromFileReader(file_reader, password);
        if (error == foxit.common.ErrorCode.e_ErrSuccess)
        {
            UpdatePageView(this.Width, this.Height, 0);
        }
        if (completion != null)
        {
            completion(error);
        }
    }

    public void CloseDoc(Cleanup cleanup)
    {
        if (current_doc_ != null)
        {
            current_doc_.Dispose();
            current_doc_ = null;
        }

        if (cleanup != null)
        {
            cleanup();
        }
    }

    public void SaveDoc(string file_path, int flag)
    {
        if (current_doc_ != null)
            current_doc_.SaveDoc(file_path, flag);
    }

    public void SaveDocToFileWriter(FileWriterCallback file_write, int flag)
    {
        if (current_doc_ != null)
            current_doc_.SaveDocToFileWriter(file_write, flag);
    }

#endregion

    public int GetPageCount()
    {
        if (current_doc_ != null)
            return current_doc_.GetPageCount();
        else
            return 0;
    }

    public int GetCurrentPage()
    {
        if (current_doc_ != null)
            return current_doc_.GetCurrentPage();
        else
            return 0;
    }

    public int[] GetVisiblePages()
    {
        if (current_doc_ != null)
            return current_doc_.GetVisiblePages();
        else
            return new int[] { 0 };
    }

    public bool IsPageVisible(int page_index)
    {
        if (current_doc_ != null)
            return current_doc_.IsPageVisible(page_index);
        else
            return false;
    }

    public float GetZoom()
    {
        return zoom_factor_;
    }

    public void SetZoom(float zoom)
    {
        zoom_factor_ = zoom;
        if (current_doc_ == null) { return; }
        UpdatePageView(this.Width, this.Height, current_doc_.GetCurrentPage());
    }

    public void RotateView(int rotation)
    {
        throw new NotImplementedException();
    }

    public int GetViewRotation()
    {
        throw new NotImplementedException();
    }

    public bool GotoPage(int page_index)
    {
        if (current_doc_ == null) { return false; }
        if (page_index < 0 || page_index >= current_doc_.GetPageCount())
        {
            return false;
        }
        current_doc_.SetCurrentPage(page_index);
        UpdatePageView(this.Width, this.Height, current_doc_.GetCurrentPage());
        ScrollToAsync(0, 0, false);
        return true;
    }

    public bool GotoFirstPage()
    {
        if (current_doc_ == null) { return false; }
        current_doc_.SetCurrentPage(0);
        UpdatePageView(this.Width, this.Height, 0);
        ScrollToAsync(0, 0, false);
        return true;
    }

    public bool GotoLastPage()
    {
        if (current_doc_ == null) { return false; }
        current_doc_.SetCurrentPage(current_doc_.GetPageCount() - 1);
        UpdatePageView(this.Width, this.Height, current_doc_.GetPageCount() - 1);
        ScrollToAsync(0, 0, false);
        return true;
    }

    public bool GotoNextPage()
    {
        if (current_doc_ == null) { return false; }
        int page_index = current_doc_.GetCurrentPage();
        if(page_index >= current_doc_.GetPageCount() - 1)
        {
            return false;
        }
        current_doc_.SetCurrentPage(page_index+ 1);
        UpdatePageView(this.Width, this.Height, current_doc_.GetCurrentPage());
        ScrollToAsync(0, 0, false);
        return true;
    }

    public bool GotoPrevPage() {
        if (current_doc_ == null) { return false; }
        int page_index = current_doc_.GetCurrentPage();
        if (page_index <= 0)
        {
            return false;
        }
        current_doc_.SetCurrentPage(page_index - 1);
        UpdatePageView(this.Width, this.Height, current_doc_.GetCurrentPage());
        ScrollToAsync(0, 0, false);
        return true;
    }

}
