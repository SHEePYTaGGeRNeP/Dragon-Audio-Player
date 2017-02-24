using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread itemThread;
        private BindExample _be = new BindExample();

        public MainWindow()
        {
            InitializeComponent();
            this.lvData.ItemsSource = this._be.Bind;
            this.Initialise();
        }

        private void Initialise()
        {
            itemThread = new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    i++;
                    this._be.Bind.Add(new BindClass() { Name = "Hello", Name2 = " test", ModifiedDate = DateTime.Now });
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        this.lvData.Items.Refresh();
                    }));
                    Thread.Sleep(500);
                    if (i > 5)
                    {
                        i = 0;
                        this._be.Bind.Clear();
                    }
                }
            });
            itemThread.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            itemThread.Abort();
        }
    }
}
