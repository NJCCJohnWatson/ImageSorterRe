using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ImageRe
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Storage.Pickers.FileOpenPicker imgpicker = new Windows.Storage.Pickers.FileOpenPicker();
        Windows.Storage.Pickers.FolderPicker imgfpik = new Windows.Storage.Pickers.FolderPicker();
        Windows.Storage.StorageFile imagefile;
        Windows.Storage.StorageFolder imagefolder;
        IReadOnlyList<StorageFile> imgfiles;
        Random ran = new Random();
        StringBuilder stringBuilder;
        BitmapImage horbitmap = new BitmapImage();
        BitmapImage verbitmap = new BitmapImage();
        IRandomAccessStream inputstream;
        public MainPage()
        {
            this.InitializeComponent();
            btnSat.IsEnabled = false;
        }

        private async void OpenImageFolder(object sender, RoutedEventArgs e)
        {
            //imgpicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //imgpicker.FileTypeFilter.Add(".jpg");
            //imgpicker.FileTypeFilter.Add(".jpeg");
            //imgpicker.FileTypeFilter.Add(".png");
            //imgpicker.FileTypeFilter.Add(".bmp");
            //imgpicker.FileTypeFilter.Add(".gif");
            //imagefile = await imgpicker.PickSingleFileAsync();
            //if (imagefile == null) return;
            //Tot.Text = imagefile.ContentType.ToString();
            //if (imagefile == null) return;
            //IRandomAccessStream inputstream = await imagefile.OpenReadAsync();
            //ImageProperties imageProperties = await imagefile.Properties.GetImagePropertiesAsync();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.SetSource(inputstream);
            //ImagePlace.Source = bitmap;
            btnOpn.IsEnabled = false;
            imgfpik.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            imgfpik.FileTypeFilter.Add(".jpg");
            imgfpik.FileTypeFilter.Add(".jpeg");
            imgfpik.FileTypeFilter.Add(".png");
            imgfpik.FileTypeFilter.Add(".bmp");
            imagefolder = await imgfpik.PickSingleFolderAsync();
            if (imagefolder == null)
            {
                btnOpn.IsEnabled = true;
                return;
            }
            imgfiles = await imagefolder.GetFilesAsync();
            if (imgfiles.Count < 1)
            {
                btnOpn.IsEnabled = true;
                return;
            }
            int rankey = ran.Next(0, imgfiles.Count);
            imagefile = imgfiles.ElementAt<StorageFile>(rankey);
            if (imagefile == null) return;
            Tot.Text = imagefile.ContentType.ToString();
            if (imagefile == null) return;
            inputstream = await imagefile.OpenReadAsync();
            ImageProperties imageProperties = await imagefile.Properties.GetImagePropertiesAsync();
            horbitmap.SetSource(inputstream);
            HorImagePlace.Source = horbitmap;
            btnSat.IsEnabled = true;
            VerImagePlace.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Tot.Text = "";
            Vot.Text = "";
            Button button = sender as Button;
            button.IsEnabled = false;
            StorageFolder vere = imagefolder;
            StorageFolder hore = imagefolder;
            if (imgfiles == null) return;
            //Tot.Text = imagefile.Path;
            Tot.Text = imagefile.FileType;
            //若存在则获取，不存在则新建
            if (null == await imagefolder.TryGetItemAsync("Hor")) hore = await imagefolder.CreateFolderAsync("Hor");
            hore = await imagefolder.GetFolderAsync("Hor");
            if (await imagefolder.TryGetItemAsync("Ver") == null) vere = await imagefolder.CreateFolderAsync("Ver");
            vere = await imagefolder.GetFolderAsync("Ver");
            int a = 0;
            int b = 0;
            float t = 0;
            float p = 0;
            double c = 0;
            Tot.Text = a.ToString();
            Vot.Text = b.ToString();
            //Windows.Storage.StorageFolder imgfld = await imagefile.GetParentAsync();
            StorageApplicationPermissions.FutureAccessList.Add(imagefolder);
            IReadOnlyList<StorageFile> filelist = await imagefolder.GetFilesAsync();
            foreach (StorageFile file in filelist)
            {
                progrb.Visibility = Visibility.Visible;
                p = filelist.Count;
                if (file.FileType == ".jpg" || file.FileType == ".png" || file.FileType == ".bmp")
                {
                    ImageProperties imageProperties = await file.Properties.GetImagePropertiesAsync();
                    if (imageProperties.Width >= imageProperties.Height)
                    {
                        if (await hore.TryGetItemAsync(file.Name) != null) return;
                        inputstream = await file.OpenReadAsync();
                        await horbitmap.SetSourceAsync(inputstream);
                        HorImagePlace.Source = horbitmap;
                        await file.CopyAsync(hore);
                        a++;
                    }
                    else
                    {
                        if (await vere.TryGetItemAsync(file.Name) != null) return;
                        inputstream = await file.OpenReadAsync();
                        await verbitmap.SetSourceAsync(inputstream);
                        VerImagePlace.Visibility = Visibility.Visible;
                        VerImagePlace.Source = verbitmap;
                        await file.CopyAsync(vere);
                        b++;
                    }
                    Tot.Text = a.ToString();
                    Vot.Text = b.ToString();
                }
                Tot.Text += " Hor Pictures Copyed";
                Vot.Text += " Ver Pictures Copyed";
                button.IsEnabled = true;
                btnOpn.IsEnabled = true;
                t++;
                c = t/p;
                progrb.Value = c*100;
            }
            StorageApplicationPermissions.FutureAccessList.Clear();
        }
    }
}
