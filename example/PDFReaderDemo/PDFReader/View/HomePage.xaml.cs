using System.Drawing.Printing;
using foxit.common;
namespace PDFReader.View;

public partial class HomePage : ContentPage
{
    private bool isLibraryInitialize = false;
    public bool IsLibraryInitialize
    {
        get
        {
            return isLibraryInitialize;
        }
    }

    public HomePage()
    {
        InitializeComponent();
        SDKInit();
        tool_view_ctrl_.SetView(pdf_view_ctrl_);
#if WINDOWS || MACCATALYST
        tool_view_ctrl_.IsVisible = false;
#endif       
    }

    private void SDKInit()
    {
        if (IsLibraryInitialize) return;

        string sn = "tAhLFPRCTzxYWNxXYFMALtaoFs8Luenwc9KEZ1IkxoywzlFkoYbdVg==";
        string key = "8f3g1fFtvR8NQze6Lm6tMXDPrRiaAK++egW7j2G5DeGHv9jVN0mC9+fXdx1erOYlzBtgm9P+/DqjuaEid3ukGXlhdGTt8CAD7RPtiHACxlOCfbaDqBEkWLo3bUkin9x9A/uvPnH6tgJiNf45qCD5VU/CdZcXBCd/q8iyEaQJ643GbWo0ucfSKNxBGCRYtNCW5XkigDEqpXI/TktIXDKXi1oLBQ4+/4px3jVOycPAZAjx+JjmkrLVVmJbjInOx37EYS3fkCrb8O1hXsc5Xbh0M/En6PdDH09yRfTihHXk468CGQvr+kesoXfTs4hQ+2yFjKof6TKswBSu+8xdAd+bohiOXG6ztbPI4OcHSMwyKYMDOb82TAsIVLFzybhxz9yD25chTkIqWoy2N49sctFY7ezBmOTGIzV0Kif2hchuKodADwbyyFXMdo84N8D3HpqWSFbrvMFcZikDl74T8YxbSoNndBIACmmQ2hHTKJpe4G9OORvqKFMsaQk2Z+9QrO36CuufKjDjZCRHT2cXjCmnLrrBLqpSmpBRppcSYxQmeJVuNcFdWRxbuwCY/JenginYYUXhe5YyiEZ/cMivT8zchqrb8HPgVqEU+H47ALiBk/a2FgMocRezRTE4FuevcC/rYW6fENFTqUD9f4tMriPprEDDTMSVkBnST47ojcN9uZg+gK8VHvyuNroOXrV/YKZUtv1985ZJ6mUp9VSBIIphl7thZmNaijQ95y3WMbG8HgrnxK/VV4+y2P+ZOGhV85ZON10DmZehAXQD3FWZTQV8mUFdKuO6n/crC9V9nXDCJXd59tNOOOxYVQS+Q66J+SGEqCLHiIep/tunKTQsG5fa+9tDAgXdsyMHVfelAQaHsau0SXj53OjnkFOgbsox8CLU8taeXk0CdZhXjvIjY7MRedE/qm0EFMR0eXw6IdpMFeIqggfKY4RVh+7xf1LoPXOXwRSXSIh6+oV1vRErirrDLlmdnKgG46hQ0O+pT4yRjjW4loLBwQKRF5w3R44NXAEuUMuL/v2ST4k1p6Zyfot74KqD7N/A1Bux8w4y64XuS7JS7uB0nS9zUM4xKSrBA3y7cioGwF8Nhrvw5CQ5L4vN7zci3hFGangp7o0+oWPB3dN5Ri07HMKKPY/bOI5Dv+xV0hlXXo00Po2//joG2XRE1hDOQjp5dOTxvhZGVsiBpHBKKfNRKSVirY/tZB6dJhVddKuokaqrf7x2DYanU9WzJ8oJmSYk01yWcVewI/tiPFyb5bUnC6jeNRX2meNby3dVWbC++m1n9L5jpFS4xZMc3V4usGNJOclPIuIdZj6SRYPaDijzMB1g+EXNN0rxgE3eOKsuiLXeW7NAlZ2vqpYlcPuUqoolJDybVfit8mN8EugTKXxgVPIRGcQ=";

        ErrorCode code = Library.Initialize(sn, key);
        if (ErrorCode.e_ErrSuccess == code)
        {
            isLibraryInitialize = true;
            tool_view_ctrl_.IsLibraryInitialize = true;
        }
        else
        {
            isLibraryInitialize = false;
            tool_view_ctrl_.IsLibraryInitialize = false;
        }

    }

    private void Completion(foxit.common.ErrorCode error)
    {
        if (error == foxit.common.ErrorCode.e_ErrSuccess)
        {
            //
        }
    }

    private async void OnOpenFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (!IsLibraryInitialize)
            {
                await DisplayAlert("Error", "Initialize Library Fail", "OK");
                return;
            }

            var custom_file_type = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                    { DevicePlatform.MacCatalyst, new[] { "pdf"} }
            });

            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
#if MACCATALYST
                FileTypes = custom_file_type
#else
                FileTypes = FilePickerFileType.Pdf
#endif
            };

            FileResult result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    pdf_view_ctrl_.OpenDoc(result.FullPath, "", Completion);
                    pdf_view_ctrl_.IsVisible = true;
                    tool_view_ctrl_.UpdatePage();
#if WINDOWS || MACCATALYST
                    tool_view_ctrl_.IsVisible = true;
#endif
                }
            }
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        pdf_view_ctrl_.CloseDoc(null);
        pdf_view_ctrl_.IsVisible = false;
        tool_view_ctrl_.IsVisible = false;
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        Application.Current.Quit();
#endif
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("About", "MAUI PDF Reader Demo Poc_v1_202303081030", "OK");
    }

}

