using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CashFlow.Controler;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CashFlow.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
            this.Loaded += BlankPage1_Loaded;
        }

        private void BlankPage1_Loaded(object sender, RoutedEventArgs e)
        {
            init();

        }

        private async void init()
        {
            Frame.BackStack.Clear();
            await MapController.Test();
            await Task.Delay(TimeSpan.FromSeconds(2));
            Frame mapFrame = new Frame();
            Window.Current.Content = mapFrame;

            mapFrame.Navigate(typeof(MapsPage));
            Window.Current.Activate();
        }
    }
}
