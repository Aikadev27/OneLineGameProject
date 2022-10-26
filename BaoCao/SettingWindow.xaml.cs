using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();


            this.Loaded += SettingWindow_Loaded;
        }


        public class MyData
        {
            public string Text { get; set; }
            public MyData(string s)
            {
                Text = s;
            }
        }

        private void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<MyData> list = new List<MyData>()
            {
                new MyData("chuoi 1"),
                new MyData("chuoi 2"),
                new MyData("chuoi 3"),
                new MyData("chuoi 4"),
                new MyData("chuoi 5"),
                new MyData("chuoi 6"),
                new MyData("chuoi 7"),
            };

                        
            cbxLevels.ItemsSource = list;
            cbxLevels.DisplayMemberPath = "Text";
        }
    }
}
