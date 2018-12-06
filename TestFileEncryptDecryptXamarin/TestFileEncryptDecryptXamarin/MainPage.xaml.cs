using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.XamarinFileEncryptorDecryptor;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using PCLStorage;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System.Text.RegularExpressions;

namespace TestFileEncryptDecryptXamarin
{
	public partial class MainPage : ContentPage
	{
        string localPath, roamingPath, documentPath, libraryPath, localAppDataPath, commonAppDataPath, commonDocPath;
        string androidSDPath;
        Assembly assembly;
        SharedPopup popup;

        public MainPage()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            popup = new SharedPopup();            
            //popup.IsLoading = true;
            //popup.ToggleTemp = true;
            //For using application directory - no direct access to user
            localPath = XamEncDec.GetLocalPath();

            //For using roaming directory - not all platform support this
            roamingPath = XamEncDec.GetRoamingPath();

            //For using external storage path in Android
            androidSDPath = "";
            if (Device.RuntimePlatform == Device.Android)
                androidSDPath = CrossXamarinFileEncryptorDecryptor.Current.GetAndroidExternalStoragePath();

            //For using shared document path - commonly for iOS
            documentPath = XamEncDec.GetDocumentsPath();

            //For using library path in iOS
            libraryPath = "";
            if (Device.RuntimePlatform == Device.iOS)
                libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");

            //Other paths that you can use, theres more if you google
            localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            commonDocPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            //getting the assembly to access resource files
            assembly = IntrospectionExtensions.GetTypeInfo(typeof(TestFileEncryptDecryptXamarin.App)).Assembly;
        }        

        public async void EncryptResourceFileToBase64String(Assembly assembly, string resourceName, string password)
        {
            //eg. encrypt resource file and generate encrypted Base64 string

            //XamEncDec.EncryptResourceFileAsBase64String(<Assembly object>, 
                                                    //<Resource filename>, <password>);
            var encryptedString = await XamEncDec.EncryptResourceFileAsBase64String(assembly, "TestFileEncryptDecryptXamarin.Files.MARBLES.JPG", "myPassword");
        }       

        public async void EncryptResourceFileToNewFile()
        {
            //eg. encrypt resource file and generate a new encrypted file 

            //XamEncDec.EncryptResourceFileAsNewFile(<Assembly object>, <Resource filename>, 
                        //<True=local directory,False=use custom path>,<set string for custom path>,
                                                    //<new/existing folder name>,<new filename>, <password>);
            var isFileEncrypted = await XamEncDec.EncryptResourceFileAsNewFile(assembly, "TestFileEncryptDecryptXamarin.Files.MARBLES.JPG",true,"","folderName1","MARBLES.JPG", "myPassword");
        }

        public async void EncryptFileToBase64String()
        {
            //eg. encrypt a file and generate encrypted base64 string

            //XamEncDec.EncryptFileAsBase64String(<true/false to delete source file>, 
            //<True=local directory,False=use custom path>, <set string for custom path>, 
            //<new/existing folder name>, <file name to be encrypted>, <password for encryption>);
            var encryptedString = await XamEncDec.EncryptFileAsBase64String(false, true, "", "folderName2", "MARBLES.JPG", "myPassword");
        }

        public async void EncryptFileToNewFile()
        {
            //eg. encrypt a file and generate new encrypted file

            //XamEncDec.EncryptFileAsNewFile(<true/false to delete source file>, 
            //<True=local directory,False=use custom path>, <set string for custom path>, 
            //<new/existing folder name>, <file name to be encrypted>, <password for encryption>,
            //<new file name for encrypted file>);
            bool isFileEncrypted = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                isFileEncrypted = await XamEncDec.EncryptFileAsNewFile(false, false,
                    CrossXamarinFileEncryptorDecryptor.Current.GetAndroidExternalStoragePath(), 
                    "folderName1", "MARBLES.JPG", "myPassword", "MARBLES_ENCRYPTED.JPG");                    
            }
            
        }

        public async void DecryptEncryptedString(string encryptedString)
        {
            //eg. decrypt encrypted Base64 string

            //XamEncDec.DecryptEncryptedStringAsBase64String(<encrypted string>, <password used for encryption>);
            var decryptedString = await XamEncDec.DecryptEncryptedStringAsString(encryptedString, "myPassword");
        }

        public async void DecryptEncryptedStringToNewFile(string encryptedString)
        {
            //eg. decrypt encrypted string to a new file

            //XamEncDec.DecryptEncryptedStringAsNewFile(<True=local directory,False=use custom path>, 
            //<set string for custom path>, <folder name for output>, <encrypted Base64 string>, 
            //<new filename for decrypted file>, <password used for encryption>);
            var isFileDecrypted = await XamEncDec.DecryptEncryptedStringAsNewFile(false, androidSDPath, "test7", encryptedString, "test1.jpg", "myPassword");
        }

        public async void DecryptEncryptedFileToNewFile()
        {
            //eg. decrypt encrypted file to a new file

            //XamEncDec.DecryptFileAsNewFile(<True=local directory,False=use custom path>, 
            //<set string for custom path>, <folder name for output>, <encrypted filename>, 
            //<password used for encryption>, <new filename for decrypted file>);
            var isFileDecrypted = await XamEncDec.DecryptFileAsNewFile(true, "", "folderName1", "MARBLES_ENCRYPTED.JPG", "myPassword", "MARBLES_DECRYPTED.JPG");
        }

        private async void EncryptResFileToBase64String_Clicked(object sender, EventArgs e)
        {
            if (p_resfile1.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please select resource file", "OK");
            }
            else
            {
                await Navigation.PushModalAsync(popup);
                await Task.Delay(5000);
                string encryptedStr = await XamEncDec.EncryptResourceFileAsBase64String(assembly, GetResourceValue(p_resfile1.SelectedIndex), "myPassword");
                if(encryptedStr != "")
                {
                    //edt_encrestostr.Text = encryptedStr;
                    Output outp = new Output();
                    outp.OutputStr = encryptedStr;
                    var OutputPage = new OutputText();
                    OutputPage.BindingContext = outp;
                    await Navigation.PushAsync(OutputPage);
                }
                else
                {
                    await DisplayAlert("Error", "Resource file failed to encrypted", "OK");
                }
                await Navigation.PopModalAsync();
            }
        }

        private async void EncryptResFileToNewFile_Clicked(object sender, EventArgs e)
        {
            if(p_resfile2.SelectedIndex == -1)
            {
                await DisplayAlert("Error","Please select resource file","OK");
            }
            else
            {
                if(p_restofile.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Please select path for generating encrpyted file", "OK");
                }
                else
                {
                    if(ent_restofile_filename.Text == "")
                    {
                        await DisplayAlert("Error", "Please set the filename", "OK");
                    }
                    else
                    {
                        await Navigation.PushModalAsync(popup);
                        await Task.Delay(5000);
                        bool isFileEncrypted = await XamEncDec.EncryptResourceFileAsNewFile(assembly, 
                            GetResourceValue(p_resfile2.SelectedIndex), 
                            UseLocalDirectory(p_restofile.SelectedIndex), 
                            SetPath(p_restofile.SelectedIndex), 
                            ent_restofile_foldername.Text, ent_restofile_filename.Text, "myPassword");
                        await Navigation.PopModalAsync();
                        if (isFileEncrypted)
                        {
                            await DisplayAlert("Encrypt", "File successfully encrypted", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Encryption failed", "OK");
                        }
                    }
                }
            }

        }

        private async void EncryptFileToBase64String_Clicked(object sender, EventArgs e)
        {
            FileData file = await CrossFilePicker.Current.PickFile();

            if (file != null)
            {
                await Navigation.PushModalAsync(popup);
                await Task.Delay(5000);                
                StringBuilder stb = new StringBuilder(file.FilePath);
                int postIndex = IndexOf(stb, file.FileName, 0, true);
                string folderPath = stb.Remove(postIndex, file.FilePath.Length - postIndex).ToString();

                //convert android content Uri into physical path, normally happens in Android emulator
                folderPath = ConvertAndroidContentUriPath(folderPath);

                string encryptedStr = await XamEncDec.EncryptFileAsBase64String(false, false, folderPath, "", file.FileName, "myPassword");
                //edt_encfiletostr.Text = encryptedStr;                
                file.Dispose();                
                Output outp = new Output();
                outp.OutputStr = encryptedStr;
                var OutputPage = new OutputText();
                OutputPage.BindingContext = outp;
                await Navigation.PushAsync(OutputPage);
                await Navigation.PopModalAsync();
            }
        }

        private async void EncryptFileToNewFile_Clicked(object sender, EventArgs e)
        {
            if(ent_filetofile_filename.Text != "" && ent_filetofile_filename.Text != null)
            {
                var file = await CrossFilePicker.Current.PickFile();

                if (file != null)
                {
                    await Navigation.PushModalAsync(popup);
                    await Task.Delay(5000);
                    StringBuilder stb = new StringBuilder(file.FilePath);
                    int postIndex = IndexOf(stb, file.FileName, 0, true);
                    string folderPath = stb.Remove(postIndex, file.FilePath.Length - postIndex).ToString();

                    //convert android content Uri into physical path, normally happens in Android emulator
                    folderPath = ConvertAndroidContentUriPath(folderPath);

                    bool isFileEncrypted = await XamEncDec.EncryptFileAsNewFile(false, false, folderPath, "", file.FileName, "myPassword", ent_filetofile_filename.Text);
                    if (isFileEncrypted)
                    {
                        await DisplayAlert("Encrypt", "File successfully encrypted", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Encryption failed", "OK");
                    }
                    file.Dispose();
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await DisplayAlert("Error", "Please set the filename", "OK");
            }
            
        }

        private string GetResourceValue(int index)
        {
            string resValue = "";
            switch(index)
            {
                case 0:
                    resValue = "TestFileEncryptDecryptXamarin.Files.MARBLES.JPG";
                break;
                case 1:
                    resValue = "TestFileEncryptDecryptXamarin.Files.PCLTextResource.txt";
                break;
            }
            return resValue;
        }

        private bool UseLocalDirectory(int index)
        {
            bool isLocal = false;

            if(index == 0)
            {
                isLocal = true;
            }

            return isLocal;
        }

        private string SetPath(int index)
        {
            string path = "";
            switch(index)
            {
                case 0:
                    path = "";
                    break;
                case 1:
                    path = androidSDPath;
                    break;
                case 3:
                    path = documentPath;
                    break;
                case 4:
                    path = libraryPath;
                    break;
                case 5:
                    path = roamingPath;
                    break;
            }

            return path;
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
