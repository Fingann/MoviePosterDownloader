using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoviePosterDownloader.TabViewModels;

namespace MoviePosterDownloader.TabViews
{
    /// <summary>
    /// Interaction logic for FolderMovieView.xaml
    /// </summary>
    public partial class FolderMovieView : UserControl
    {
        public FolderMovieView()
        {
            InitializeComponent();
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            
            var test2 = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            FileAttributes attr = File.GetAttributes(test2.First());

            if (attr.HasFlag(FileAttributes.Directory))
                (this.DataContext as FolderMovieViewModel).SelectedFolder = test2.First() + "\\";
            else
            {
                var directory = string.Empty;
                var splittedPath = test2.First().Split('\\');
                var length = splittedPath.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    directory += splittedPath[i] + "\\";
                }
                (this.DataContext as FolderMovieViewModel).SelectedFolder = directory;
            }
                


        }
    }
}
