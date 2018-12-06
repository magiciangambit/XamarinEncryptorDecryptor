using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.XamarinFileEncryptorDecryptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestFileEncryptDecryptXamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DecryptPage : ContentPage
	{
        SharedPopup popup;
        public DecryptPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            popup = new SharedPopup();
        }

        private async void DecryptFileToBase64String_Clicked(object sender, EventArgs e)
        {
            FileData file = await CrossFilePicker.Current.PickFile();

            if (file != null)
            {
                try
                {
                    await Navigation.PushModalAsync(popup);
                    await Task.Delay(5000);

                    StringBuilder stb = new StringBuilder(file.FilePath);
                    int postIndex = IndexOf(stb, file.FileName, 0, true);
                    
                    string folderPath = stb.Remove(postIndex, file.FilePath.Length - postIndex).ToString();

                    //convert android content Uri into physical path, normally happens in Android emulator
                    folderPath = ConvertAndroidContentUriPath(folderPath);

                    string encryptedStr = await XamEncDec.DecryptFileAsString(false, folderPath, "", file.FileName, "myPassword");
                    file.Dispose();

                    Output outp = new Output();
                    outp.OutputStr = encryptedStr;
                    var OutputPage = new OutputText();
                    OutputPage.BindingContext = outp;

                    await Navigation.PushAsync(OutputPage);
                    
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Error", "Decryption failed, file is not a valid encrypted file", "OK");
                }

                await Navigation.PopModalAsync();

            }
        }

        private async void DecryptFileToNewFile_Clicked(object sender, EventArgs e)
        {
            if (ent_filetofile_filename.Text != "" && ent_filetofile_filename.Text != null)
            {
                var file = await CrossFilePicker.Current.PickFile();

                try
                {
                    if (file != null)
                    {
                        await Navigation.PushModalAsync(popup);
                        await Task.Delay(5000);

                        StringBuilder stb = new StringBuilder(file.FilePath);
                        int postIndex = IndexOf(stb, file.FileName, 0, true);
                        string folderPath = stb.Remove(postIndex, file.FilePath.Length - postIndex).ToString();

                        //convert android content Uri into physical path, normally happens in Android emulator
                        folderPath = ConvertAndroidContentUriPath(folderPath);

                        bool isFileEncrypted = await XamEncDec.DecryptFileAsNewFile(false, folderPath, "", file.FileName, "myPassword", ent_filetofile_filename.Text);
                        if (isFileEncrypted)
                        {
                            await DisplayAlert("Decrypt", "File successfully decrypted", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Decryption failed", "OK");
                        }
                        file.Dispose();
                        
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Decryption failed, file is not a valid encrypted file", "OK");
                }

                await Navigation.PopModalAsync();

            }
            else
            {
                await DisplayAlert("Error", "Please set the filename", "OK");
            }

        }

        public int IndexOf(StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        private string ConvertAndroidContentUriPath(string folderPath)
        {
            StringBuilder stbCon = new StringBuilder(folderPath);
            int contentIndex = IndexOf(stbCon, "content://", 0, true);
            if (contentIndex > -1)
            {
                int androidStorageIndex = IndexOf(stbCon, "com.android.externalstorage.documents/document/", 0, true);
                if (androidStorageIndex > 0)
                {
                    var physicalFolderName = Regex.Replace(folderPath, @"%2F", "");
                    int weirdSymbol = physicalFolderName.LastIndexOf(@"%3A");

                    physicalFolderName = physicalFolderName.Substring(weirdSymbol + 3);
                    folderPath = "/storage/sdcard/" + physicalFolderName;
                }
            }

            return folderPath;
        }
    }
}