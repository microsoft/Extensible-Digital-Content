using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    /// <summary>
    /// The FileNavView page is responsible for displaying the contents of a directory
    /// and handling navigation to the correct page when a file/content is clicked.
    /// The directory is passed in as a parameter and is managed by the FileNavViewModel.
    /// </summary>
    public sealed partial class FileNavView : Page
    {
        private FileNavViewModel _viewModel;
        public FileNavView()
        {
            this.InitializeComponent();

            _viewModel = new FileNavViewModel();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// OnNavigatedTo takes a FolderContent object as a parameter and passes the path to the FileNavViewModel.
        /// </summary>
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            Content clickedContent = (Content)e.ClickedItem;
            this.Frame.Navigate(clickedContent.SourcePageType, clickedContent);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.PopulateContent(((FolderContent)e.Parameter).Path);
            SharedUIViewModel.Instance.CurrentViewTitle = ((FolderContent)e.Parameter).Path;
            base.OnNavigatedTo(e);
        }
    }
    
}
