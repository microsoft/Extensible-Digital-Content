using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    public sealed partial class FileNavView : Page
    {
        private FileNavViewModel _viewModel;
        public FileNavView()
        {
            this.InitializeComponent();

            _viewModel = new FileNavViewModel();
            this.DataContext = _viewModel;
        }

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
