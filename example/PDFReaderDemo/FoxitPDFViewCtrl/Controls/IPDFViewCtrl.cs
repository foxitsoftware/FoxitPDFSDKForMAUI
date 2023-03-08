using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using foxit.common.fxcrt;
using foxit.pdf;


namespace FoxitPDFViewCtrl.Controls
{
    public interface IPDFViewCtrl
    {
        public void SetDoc(PDFDoc doc);
        public PDFDoc? GetDoc();
        public void OpenDoc(string filepath, string password, Completion completion);

        public void OpenDocFromMemory(byte[] buffer, string password, Completion completion);

        public void OpenDocFromFileReader(FileReaderCallback file_reader, string password, Completion completion);

        public void CloseDoc(Cleanup cleanup);

        public void SaveDoc(string file_path, int flag);

        public void SaveDocToFileWriter(FileWriterCallback file_write, int flag);

        public int GetPageCount();

        public int GetCurrentPage();

        public int[] GetVisiblePages();

        public bool IsPageVisible(int page_index);

        public float GetZoom();

        public void SetZoom(float zoom);

        public void RotateView(int rotation);

        public int GetViewRotation();

        public bool GotoPage(int index);

        public bool GotoFirstPage();

        public bool GotoLastPage();

        public bool GotoPrevPage();
    }
}
