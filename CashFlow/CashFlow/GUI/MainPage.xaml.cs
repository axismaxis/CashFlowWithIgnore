using CashFlow.Acount;
using CashFlow.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CashFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile myfile;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += SignUpPage_Loaded;
        }

        private async void SignUpPage_Loaded(object sender, RoutedEventArgs e)
        {
            AccountInfo loadedAccountInfo = await JsonSave.LoadPersonalDataFromJson();
            outputBox.Text = loadedAccountInfo.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            AccountInfo info = new AccountInfo("Kerk", 100.0, 4.7812310, 51.5851460);
            JsonSave.SavePersonalDataToJson(info);
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            AccountInfo loadedAccountInfo = await JsonSave.LoadPersonalDataFromJson();
            outputBox.Text = loadedAccountInfo.ToString();
        }
    }
}
