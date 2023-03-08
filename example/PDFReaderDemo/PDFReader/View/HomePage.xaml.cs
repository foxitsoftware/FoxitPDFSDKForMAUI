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

        string sn = "LcftD1mEXpXKhcdRyBB3Ydszmi4eDfYC3M2U7i6N+ppsaH+lm1ZNMg==";
        string key = "8f3g1fFtvR8NQze6Lm6tMSBzJHOt3ek7G5vjdwc8pbWN4bRWVS2H6u8gQadT+fcsR70GewX+yXGWrQE7/oCikvEM6fCR4de1LjUyu6VglHEaJHYkpUEYqBJWSUCYmRlsD4swvf2EdHJ4GDWIJVAiypkabTA2DL7ehqPnRCzoWSWX/bPm2LKc46Cg9apauYxh6gEttfAb//SkxmKkuJ3gkL6yTyoXtBYpWXlPaVyIsRaZ8UTNRqk3sQv4nU7YprcqWH7b52ee7j5X3Gs6KOvOXiXJiV9CGxZx5hEESe3PBQZDAivQ+5EbuRZ0F99iyKNVYbyVQ9yPN6DIzFkErhsAph0NS03CPy3ChIV7H3BXTE2hcwmgJI7QzwCAV70pXhEHum5pXj6tY+YDwHn8H20mdbWjLuFbbG6ES8D5Iyw4/285f0qCSakI5IUDBfw4fK9YQjgWiI2GZnORys6UHvKF64rNZQmTpR0dvs/5lkEsJAEGugukw4+I/Ls/BW5K70kWP2SaWw5eF7ujtkgtg1mO9OS+ORRNd7U/KnThejyolKJSK19T/0lU+4l6T1zGDZ5G/3kNUw5R3kXAQlMoMH4ZPiSqWNyR5EaL0OpyKxYhEjkbfPHOHkqbRAHodlGmn0d8mbRKkPkMSj1meLwuCdiP0G+XhO66nlgAXU5KgPIw869BfewekNRq2if+2cGfpkDJp77ffZ9rKmhUhjwX4jnllnvUGL5f+Mh0TMndAWBWmS+8qN9kX0jlYHr9xzyT3BIJD3ABqNfko0f3fJjSiUTYP7oUFIQsx+TdOEdZ6DATMipGVQ1Dk8RE4cixOEC6zTSay10WRTtxftAT8iuE9xHBaCuK0qY0kH8YTfC4mFej5NL8vEU2szqNULTAZX9zJRaFWDwi+wZhTu23uSM1yv2iOhXX/u+KH7AgCHQHuBRkeK9fgPHEhcRQBql549+YFKJt1N2ZV9poZuFKIsey3dbKCGD5tc/rHWbS6kygmXdsim2iCNwzw3u1Oi3/VByoI7tEjpkBvQIQzQ4fl9VmgFLXOoIKgYVY1PX5xoFXPpot/6V9YjmG5KQJyxH9CId1Exzbz0PU5wDu8V9u1WWf0G8Xm0eZffxI7rHduRciWPrFYbdJn0zlQ41kb9c7IAjZx7W0j7xj4zXYSST34xBEY5LrDsdWQWnztJpnhfVtoY0ybN3k42KLpyTh4UA/mnXghMZTitB1OVgQTV97R6BMRlr30HoGf30RUkqt+9OxFwsKjOrDxX7hPhgHB+RXGb63aEmfNy5m6eMCmhAPv+z8ALxs5apaK+BwZ4iv5iWVfLkXZdkbPkWMpYbDbdyrUPDJ4Z2jsCGO8wkUdMR4jR1TFBjN2tHfUYN5GABSVPl/dIpntUBrJjhALFviWQ==";

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

