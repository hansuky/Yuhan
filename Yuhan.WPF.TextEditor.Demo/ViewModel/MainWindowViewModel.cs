using System.Windows.Input;

namespace Yuhan.WPF.TextEditor.Demo
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        // Property variables
        private string p_DocumentXaml;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            this.LoadDocument = new LoadDocumentCommand(this);
        }

        #endregion

        #region Command Properties

        /// <summary>
        /// Command to simulate loading a document from the app back-end
        /// </summary>
        public ICommand LoadDocument { get; set; }

        #endregion

        #region Data Properties

        /// <summary>
        /// The text from the FsRichTextBox, as a XAML markup string.
        /// </summary>
        public string DocumentXaml
        {
            get { return p_DocumentXaml; }

            set
            {
                p_DocumentXaml = value;
                base.RaisePropertyChangedEvent("DocumentXaml");
            }
        }

        #endregion
    }
}