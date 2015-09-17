using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PullToRefreshListView
{
    public sealed partial class SamplePage : Page
    {
        ObservableCollection<string> Data;
        
        public SamplePage()
        {
            this.InitializeComponent();
            Data = new ObservableCollection<string>(GetData());
            lv.ItemsSource = Data;
        }

        private async void lv_RefreshContent(object sender, EventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            for (int i = 0; i < 10; i++)
            {
                Data.Insert(0, "New One");
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void lv_MoreContent(object sender, EventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            for (int i = 0; i < 10; i++)
            {
                Data.Add("Old One");
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        public static List<string> GetData()
        {
            List<string> data = new List<string>();
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                data.Add(random.Next(1, 500).ToString());
            }
            return data;
        }
    }
}
