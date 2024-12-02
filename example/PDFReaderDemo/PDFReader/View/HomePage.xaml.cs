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

        string sn = "FL9nVFrA6evbVzFbDXqAyOtQL1lpdYUeCWLr77U1zKm9cl57LSCj/Q==";
        string key = "8f3g1kNstBkNAgfCSIczugpg+pDn+5jZC8/ZLrYF6b2LMfDGYpOIzWdhlFeMkgkXzH12axq+fqW3jXEFc0ekGXk1bM1Q4de1Ljtyu9VgiHEaZDZUpXk4iFJWiWleurZmG5uZO2gndMJfFf45r6Ae8PRq6AQnuPT/hqPjBSzoqyV17TPGGLKcY4mzMoKgR+eO8pC1g3EmeHQhRGmqAfa8Bae1xAA8/4px3AtOycPAZA7xHOftkrI1dguAlCG+DdJBsVvXUl5ifX94OKQ5ucTVi+NmzW1FzvFV1MtjZWZIYu5Efxph3zq5DlYdbStf6kZ6NZYBtmz3wQvPVDbrTUy5ToTPgIkp/Nx0S9T6+v9i0QPzOGoh2sdq+Ww9+LtAe+WrqqxuQnPMGpmqhR4gvbfYhoesXssZu0Q4PcmmdLC3UlfIJ9IpDe3dXWg5q+U7x3gjDyBDDkm0vKddQYzK5UlysZB2sY4HMzFLTJbU4WT8FjozYEwXaZ+t1coBqdu3Ro76dWmOZk7oPiuSdaWD1rujZ/+QYFJF5CLfNIDgjBST+tVCFimoESrB6q3RVOpAOHFlLkkj9JGP1hCTN0pjImP+0VFePSledKnQWQobvz0rWsRrSpA1DqTlCBmKRtMBP8Xei0exFmixW140Sht7bHC8yDUntni3vqsXUu8O+fzN0N0otUEYudD32xgtmBz9mzAlmsud49wqi3VCVNZBWBwKaLXxaR0bWwDB8gT8ZfpkdZEQEx/bB+XlQbQF61Enbl7pTERGucp3a04GmheS8NRVL8E5OivTt3b8303V4fgqgUAk+3kWATJSOdAqFWZ2w1jA24MLwh7U61Qo8PJjAfpZ/9TQJ8BaJ1wGECli6AkRsOO7UPiajXzDyx0Hawke5lckQXJbkbWZwevgGgErLmnqdMQveZJFuSguY8XrIn57MLNiVyjEDsm8mZQdwa39nE6sMuSE7zxqN90x3Br9sdgiJDFfJ3ukhq8EI/gz8teMYZszRjcZNdbnOgRbliTYDknRYN/bP9LSS1J7QDCIBbPyGCoFaIMCyTyIQ6imxeqcs3mapoMZ5lUaRzLFN0J9puR9r5tTsFVppg+2ekQkomAKqylubvAhpeZZP1R/LlhkuBcEiL7vMhDnBZ52vZbk5a0RfxeKGkPzRlijhWxONrfK9EK4MPAg4x7BVm+DnfHpKTN2YcDOh/40X0CMUDZm9cpmnRjL6avNZmAynYblknaMGGTJQdBKO+cHDJTkjZCNv3bJe+WCEqNFfRaMopbPFsaWt9UxXY/poVMMsAz230/bP5NkeAIGbREZSTbnVQ9EHpHVAfeBQWrGqyhljnY97ufGc6x+V3MLmN1BbVNbT/Gn0gA1TzHGJqMiTJ/3p9QpoJEWaS7hCe8shTV+Wdzxtc3Z3ZnaRyBIB8DurQlidU7PZ543mt4Yuvs6xA==";

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

