using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MoviePoster.Core.SavePoster;
using MoviePosterDownloader.Model;
using TheMovieDatabaseHandler.Pipeline;

namespace MoviePosterDownloader.TabViewModels
{
    public class FolderMovieViewModel : ViewModelBase
    {
        private ObservableCollection<FolderPoster> _folderPosters =
            new  ObservableCollection<FolderPoster>();

        private string _selectedFolder = "";
        private bool _saveEnabled;

        public FolderMoviePipeline FolderMoviePipeline { get; set; } 
        public ITargetBlock<(string movieName,string folderPathc)> Pipe { get; set; }  

        public string SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                StartPipeline(value);
                RaisePropertyChanged();
                
            }
        }

        public bool SaveEnabled
        {
            get { return _saveEnabled; }
            set
            {
                _saveEnabled = value;
                RaisePropertyChanged();
            }
        }
        public RelayCommand SaveMoviePosterCommand { get; set; }

        private void ExecuteSaveMoviePoster()
        {

          
            Pipe.Complete();
            foreach (var poster in FolderPosters)
            {
                PosterSaver.SavePoster(poster.Directory, poster);
            } 
        }
        public FolderMovieViewModel()
        {
            FolderMoviePipeline = new FolderMoviePipeline();
            Pipe = FolderMoviePipeline.CreateMovieProcessingNetwork();

            SaveMoviePosterCommand = new RelayCommand(ExecuteSaveMoviePoster);

        }
        private void StartPipeline(string mainFolder)
        {
            FolderPosters = new ObservableCollection<FolderPoster>();
            FolderMoviePipeline.FolderMovieFinished += str => FolderPosters.Add(str);

            var directory = new DirectoryInfo(mainFolder);
                    var directories = directory.GetDirectories();
            var updater = Parallel.ForEach(directories, (movie) =>
            {
                Pipe.Post((movie.Name, movie.FullName));
            });
            SaveEnabled = true;
        }

     
        public ObservableCollection<FolderPoster> FolderPosters
        {
            get => _folderPosters;
            set
            {
                _folderPosters = value;
               
                RaisePropertyChanged();
            }
        }

    }
}

//public class FolderMovieViewModel : ViewModelBase
//{
//    private ObservableCollectionWithItemNotify<FolderPoster> _folderPosters =
//        new ObservableCollectionWithItemNotify<FolderPoster>();

//    private string _selectedFolder = "Drag folder to select path";
//    public MovieHandler MovieHandler = new MovieHandler();

//    public string SelectedFolder
//    {
//        get => _selectedFolder;
//        set
//        {
//            _selectedFolder = value;
//            BackgroundWorker.RunWorkerAsync(value);
//            //getSubFolders(value);
//            RaisePropertyChanged();
//        }
//    }

//    public FolderMovieViewModel()
//    {
//        BackgroundWorker = new BackgroundWorker();
//        BackgroundWorker.WorkerReportsProgress = true;
//        BackgroundWorker.DoWork += BackgroundWorker_DoWork;
//        BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
//    }

//    private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
//    {
//        FolderPosters.Add(e.UserState as FolderPoster);
//    }

//    private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
//    {

//        var directory = new DirectoryInfo(e.Argument as string);
//        var directories = directory.GetDirectories();
//        var updater = Parallel.ForEach(directories, (movie) =>
//        {
//            var poster = new FolderPoster(movie.Name, movie.FullName);
//            var picture = GetPictures(poster);

//            poster.PosterLink = picture.posterLink;
//            poster.PosterBytes = picture.posterBytes;
//            ((BackgroundWorker)sender).ReportProgress(1, poster);
//        });

//        while (!updater.IsCompleted)
//        {
//            Task.Delay(100);
//        }

//    }




//    public BackgroundWorker BackgroundWorker { get; set; }

//    public ObservableCollectionWithItemNotify<FolderPoster> FolderPosters
//    {
//        get => _folderPosters;
//        set
//        {
//            _folderPosters = value;
//            RaisePropertyChanged();
//        }
//    }

//    //public async void getSubFolders(string path)
//    //{
//    //    var directory = new DirectoryInfo(path);
//    //    var directories = directory.GetDirectories();

//    //    foreach (var folder in directories)
//    //        FolderPosters.Add(new FolderPoster(folder.Name, folder.FullName));
//    //    await GetPictures(FolderPosters);
//    //}

//    public (string posterLink, byte[] posterBytes) GetPictures(FolderPoster folderPosters)
//    {

//        var PosterLink =
//            MovieHandler.PosterFinder.FindPosterLinks(folderPosters.MovieName.RemoveFragmentsBetween('(', ')'));
//        var PosterBytes = MovieHandler.PosterDownloader.GetImageBytes(PosterLink);

//        return (PosterLink, PosterBytes);

//    }