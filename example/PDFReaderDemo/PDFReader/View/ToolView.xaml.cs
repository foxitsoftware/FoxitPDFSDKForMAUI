using FoxitPDFViewCtrl.Controls;


namespace PDFReader.View;

public partial class ToolView : ContentView
{
    private WeakReference weak_pdf_view_ctrl_ = null;
    private bool is_need_picker_select_update = false;

    public bool IsLibraryInitialize { get; set; }

    public ToolView()
    {
        InitializeComponent();
        IsLibraryInitialize = false;

    }

    public void SetView(PDFViewCtrl pdf_view_ctrl)
    {
        weak_pdf_view_ctrl_ = new WeakReference(pdf_view_ctrl);
    }

    public void UpdatePage()
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            int page_count = pdf_view_ctrl.GetPageCount();
            var page_list = new List<string>();
            for (int i = 0; i < page_count; i++)
            {
                page_list.Add(string.Format("{0}/{1}", i + 1, page_count));
            }
            page_picker.ItemsSource = page_list;
            page_picker.ItemsSource = page_picker.GetItemsAsArray();
            page_picker.SelectedIndex = 0;
        }
    }

    public bool HasAviliableViewCtrl()
    {
        return weak_pdf_view_ctrl_.IsAlive;
    }

    public PDFViewCtrl GetViewCtrl()
    {
        if (weak_pdf_view_ctrl_ == null) { return null; }
        return weak_pdf_view_ctrl_.IsAlive ? weak_pdf_view_ctrl_.Target as PDFViewCtrl : null;
    }

    private void UpdatePickerIndex()
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            is_need_picker_select_update = false;
            page_picker.SelectedIndex = pdf_view_ctrl.GetCurrentPage();
        }
    }

    private void OnButtonFirstPageClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            pdf_view_ctrl.GotoFirstPage();
            UpdatePickerIndex();
        }
    }

    private void OnButtonPrePageClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            pdf_view_ctrl.GotoPrevPage();
            UpdatePickerIndex();
        }
    }

    private void OnButtonNextPageClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            if(pdf_view_ctrl.GetDoc() != null)
            {
                pdf_view_ctrl.GotoNextPage();
                UpdatePickerIndex();
            }
        }
    }

    private void OnButtonLastPageClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            if (pdf_view_ctrl.GetDoc() != null)
            {
                pdf_view_ctrl.GotoLastPage();
                UpdatePickerIndex();
            }
        }
    }

    private void CheckZoomButtom()
    {
        if(scale_slider.Value >= scale_slider.Maximum)
        {
            btn_zoom_in.IsEnabled = false;
        }
        else if(scale_slider.Value <= scale_slider.Minimum)
        {
            btn_zoom_out.IsEnabled = false;
        } else
        {
            btn_zoom_in.IsEnabled = true;
            btn_zoom_out.IsEnabled=true;            
        }
    }

    private void OnButtonZoomInClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            if (pdf_view_ctrl.GetDoc() != null)
            {
                if (scale_slider.Value + 10 < scale_slider.Maximum)
                    scale_slider.Value += 10;                
                else   
                    scale_slider.Value = scale_slider.Maximum;
            }
        }
    }

    private void OnButtonZoomOutClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            if (pdf_view_ctrl.GetDoc() != null)
            {
                if (scale_slider.Value - 10 > scale_slider.Minimum)             
                    scale_slider.Value -= 10;
                else
                    scale_slider.Value = scale_slider.Minimum;
            }
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            if (pdf_view_ctrl.GetDoc() != null)
            {
                pdf_view_ctrl.SetZoom((float)(scale_slider.Value / 100));
                CheckZoomButtom();
            }
        }
    }

    private void OnPickerSelectionChanged(object sender, EventArgs e)
    {
        if (is_need_picker_select_update)
        {
            PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
            if (pdf_view_ctrl != null)
            {
                pdf_view_ctrl.GotoPage(page_picker.SelectedIndex);
            }
        }
        is_need_picker_select_update = true;
    }

    private async void OnButtonOpenClicked(object sender, EventArgs e)
    {
        PDFViewCtrl pdf_view_ctrl = GetViewCtrl();
        if (pdf_view_ctrl != null)
        {
            try
            {
                if (!IsLibraryInitialize)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Initialize library failed", "OK");
                    return;
                }

                PickOptions options = new()
                {
                    PickerTitle = "Please select a PDF file",
                    FileTypes = FilePickerFileType.Pdf
                };

                FileResult result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        pdf_view_ctrl.OpenDoc(result.FullPath, "", null);
                        UpdatePage();
                    }
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}