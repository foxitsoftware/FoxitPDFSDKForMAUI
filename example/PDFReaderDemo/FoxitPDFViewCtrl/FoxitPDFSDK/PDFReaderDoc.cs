using foxit.addon.xfa;
using foxit.common.fxcrt;
using foxit.pdf;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FoxitPDFViewCtrl.FoxitPDFSDK;

public class PDFReaderDoc : IDisposable
{
    private const int default_page_space_ = 6;
    private int page_space_ = default_page_space_;

    private bool is_init_pages_ = false;

    private string file_path_ = string.Empty;
    private string password_ = string.Empty;

    private PDFDoc? pdf_doc_ = null;

    private int total_page_count_ = 0;
    private float real_height_ = 0.0f;
    private float real_width_ = 0.0f;
    private int current_page_index_ = 0;

    private Dictionary<int, PDFReaderPage> reader_page_list_ = new Dictionary<int, PDFReaderPage>();
    private WeakReference? _view_weakref = null;

    public PDFReaderDoc()
    {

    }

    ~PDFReaderDoc()
    {
        this.Dispose(false);
    }

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
        for (int i = 0; i < reader_page_list_.Count; i++)
        {
            PDFReaderPage readerpage = reader_page_list_[i];
            readerpage.Dispose();
        }
        reader_page_list_.Clear();
        if (pdf_doc_ != null)
        {
            pdf_doc_.Dispose();
            pdf_doc_ = null;
        }
    }

    public bool UpdatePDFPage(double width, double height, foxit.common.Rotation rotate, bool need_update_page_list)
    {
        real_height_ = 0.0f;
        real_width_ = 0.0f;

        if (pdf_doc_ == null) return false;

        if (!is_init_pages_ || need_update_page_list)
        {
            for (int i = 0; i < reader_page_list_.Count; i++)
            {
                PDFReaderPage readerpage = reader_page_list_[i];
                readerpage.Dispose();
            }
            reader_page_list_.Clear();
            real_height_ = 0;
            real_width_ = 0;
            total_page_count_ = pdf_doc_.GetPageCount();
        }

        for (int i = 0; i < total_page_count_; i++)
        {
            PDFReaderPage reader_page = (is_init_pages_ && !need_update_page_list) ? reader_page_list_[i] : new PDFReaderPage();
            PDFPage? pdf_page = null;
            float page_width = 0;
            float page_height = 0;

            pdf_page = pdf_doc_.GetPage(i);
            if (pdf_page.IsEmpty())
            {
                page_width = 0;
                page_height = 0;
            }
            else
            {
                page_width = pdf_page.GetWidth();
                page_height = pdf_page.GetHeight();
            }
            float r_width = 0;
            float r_height = 0;

#if ANDROID || IOS
            r_width = (float)(width - 10);
            r_height = (float)(r_width * page_height / page_width);
#else
            r_width = (float)(height * page_width / page_height);
            r_height = (float)height - 10f;
#endif

            DisplayRotation rota = DeviceDisplay.Current.MainDisplayInfo.Rotation;
            if (rotate == foxit.common.Rotation.e_Rotation90 || rotate == foxit.common.Rotation.e_Rotation270)
            {
                real_height_ = real_height_ + r_width;
                if (r_height > real_width_)
                    real_width_ = r_height;
            }
            else
            {
                real_height_ = real_height_ + r_height;
                if (width > real_width_)
                    real_width_ = r_width;
            }
            reader_page.SetPageHeight(r_height);
            reader_page.SetPageWidth(r_width);
            reader_page.SetPageTotalHeightWithOutSpace(real_height_);
            reader_page.SetPDFPage(pdf_page);
            reader_page.SetPageIndex(i);
            if (!is_init_pages_ || need_update_page_list)
            {
                reader_page_list_.Add(i, reader_page);
            }
        }
        is_init_pages_ = true;
        return true;
    }

    public PDFReaderPage? GetPage(int page_index)
    {
        if (page_index < 0 || page_index >= total_page_count_)
            return null;
        return reader_page_list_[page_index];
    }

    public float GetRealHeight(float zoom)
    {
        return real_height_ * zoom + (total_page_count_ + 1) * page_space_;
    }

    public float GetRealWidth(float zoom)
    {
        return real_width_ * zoom + 2 * page_space_;
    }

    public int GetPageSpace()
    {
        return page_space_;
    }

    public void SetView(ScrollView view)
    {
        _view_weakref = new WeakReference(view);
    }

    public ScrollView? GetView()
    {
        if (_view_weakref != null)
        {
            return _view_weakref.IsAlive ? _view_weakref.Target as ScrollView : null;
        } else
        {
            return null;
        }        
    }

    public void SetDoc(PDFDoc doc)
    {
        pdf_doc_ = doc;
    }

    public PDFDoc? GetDoc()
    {
        return pdf_doc_;
    }

    public foxit.common.ErrorCode OpenDoc(string inputfile_path_, string password)
    {
        file_path_ = inputfile_path_;
        password_ = password;

        pdf_doc_ = new PDFDoc(inputfile_path_);
        foxit.common.ErrorCode error_code = foxit.common.ErrorCode.e_ErrSuccess;
        error_code = pdf_doc_.Load(System.Text.Encoding.Default.GetBytes(password_ ?? ""));
        if (error_code == foxit.common.ErrorCode.e_ErrSuccess)
        {
            return error_code;
        }
        if (pdf_doc_ != null)
        {
            pdf_doc_.Dispose();
        }
        return error_code;
    }

    public foxit.common.ErrorCode OpenDocFromMemory(byte[] buffer, string password)
    {
        file_path_ = string.Empty;
        password_ = password;

        int buffer_size = buffer.Length;
        IntPtr buffer_ptr = Marshal.AllocHGlobal(buffer_size);
        Marshal.Copy(buffer, 0, buffer_ptr, buffer_size);
        pdf_doc_ = new PDFDoc(buffer_ptr, (uint)buffer_size);
        foxit.common.ErrorCode error_code = foxit.common.ErrorCode.e_ErrSuccess;
        error_code = pdf_doc_.Load(System.Text.Encoding.Default.GetBytes(password_ ?? ""));
        if (error_code == foxit.common.ErrorCode.e_ErrSuccess)
        {
            return error_code;
        }
        if (pdf_doc_ != null)
        {
            pdf_doc_.Dispose();
        }
        return error_code;
    }

    public foxit.common.ErrorCode OpenDocFromFileReader(FileReaderCallback file_reader, string password)
    {
        file_path_ = string.Empty;
        password_ = password;

        pdf_doc_ = new PDFDoc(file_reader, false);
        foxit.common.ErrorCode error_code = pdf_doc_.Load(System.Text.Encoding.Default.GetBytes(password_ ?? ""));
        if (error_code == foxit.common.ErrorCode.e_ErrSuccess)
        {
            return error_code;
        }
        if (pdf_doc_ != null)
        {
            pdf_doc_.Dispose();
        }
        return error_code;

    }

    public void SaveDoc(string file_path, int flag)
    {
        if (pdf_doc_ != null && pdf_doc_.IsEmpty())
        {
            pdf_doc_.SaveAs(file_path_, flag);
        }
    }

    public void SaveDocToFileWriter(FileWriterCallback file_writer, int flag)
    {
        if (pdf_doc_ != null && !pdf_doc_.IsEmpty())
        {
            pdf_doc_.StartSaveAs(file_writer, flag, null);
        }
    }

    public int GetPageCount()
    {
        if (pdf_doc_ != null && !pdf_doc_.IsEmpty())
        {
            return pdf_doc_.GetPageCount();
        }
        return 0;
    }

    public int GetCurrentPage()
    {
        return current_page_index_;
    }

    public void SetCurrentPage(int page_index)
    {
        current_page_index_ = page_index;
    }

    public int[] GetVisiblePages()
    {
        throw new NotImplementedException();
    }

    public bool IsPageVisible(int page_index)
    {
        throw new NotImplementedException();
    }
}