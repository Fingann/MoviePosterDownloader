using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using MoviePoster.Core.Models;
using MoviePoster.Core.SavePoster;
using TheMovieDatabaseHandler.Pipeline;

namespace MoviePosterDownloader.TabViewModels
{
    public class SingleMovieViewModel : ViewModelBase
    {
        private Poster _poster;
        private string _seachText;
        private bool _saveEnabled;

        public bool SaveEnabled 
        {
            get { return _saveEnabled; }
            set { _saveEnabled = value;
                RaisePropertyChanged(); }
        }

        //Commands
        public RelayCommand<string> SearchForMovieCommand { get; set; }
        public RelayCommand<string> SaveMoviePosterCommand { get; set; }

        public SingleMoviePipeline SingleMoviePipeline { get; set; }
        public ITargetBlock<string> Pipe { get; set; }

        public SingleMovieViewModel()
        {
            SingleMoviePipeline = new SingleMoviePipeline();
            Pipe = SingleMoviePipeline.CreateMovieProcessingNetwork();
            SearchForMovieCommand = new RelayCommand<string>(ExecuteSearchForMovie);
            SaveMoviePosterCommand = new RelayCommand<string>(ExecuteSaveMoviePoster);
        }

        public Poster Poster
        {
            get => _poster;
            set
            {
                _poster = value;
                if (_poster?.PosterBytes != null)
                {
                    SaveEnabled = true;
                }
                RaisePropertyChanged();
            }
        }
        public string SeachText
        {
            get => _seachText;
            set
            {
                _seachText = value;
                RaisePropertyChanged();
            }
        }

        private void ExecuteSaveMoviePoster(string obj)
        { 
            var open = new SaveFileDialog();
            open.FileName = obj;
            open.DefaultExt = ".jpg"; // Default file extension

            var result = open.ShowDialog();

            if (result == true)
            {
                var filepath = open.FileName; // Stores Original Path in Textbox   
                PosterSaver.SavePoster(filepath, Poster);
               
            }
        }

       

        private  void ExecuteSearchForMovie(string obj)
        {
           

            SingleMoviePipeline.SingleMovieFinished += str => Poster = str;

            Pipe.Post(obj);
            
        }
    }
}