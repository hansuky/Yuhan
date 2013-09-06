using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Yuhan.WPF.TextEditor
{
    /// <summary>
    /// Interaction logic for FsRichTextBox.xaml
    /// </summary>
    public partial class RichTextEditor : UserControl
    {
        #region Fields

        // Static member variables
        private static ToggleButton m_SelectedAlignmentButton;
        private static ToggleButton m_SelectedListButton;

        // Member variables
        private int m_InternalUpdatePending;
        private bool m_TextHasChanged;

        #endregion

        #region Dependency Property Declarations

        // CodeControlsVisibility property
        public static readonly DependencyProperty CodeControlsVisibilityProperty =
            DependencyProperty.Register("CodeControlsVisibility", typeof(Visibility),
            typeof(RichTextEditor));

        // Document property
        public static readonly DependencyProperty DocumentProperty = 
            DependencyProperty.Register("Document", typeof(FlowDocument),
            typeof(RichTextEditor), new PropertyMetadata(OnDocumentChanged));

        // ToolbarBackground property
        public static readonly DependencyProperty ToolbarBackgroundProperty = 
            DependencyProperty.Register("ToolbarBackground", typeof(Brush),
            typeof(RichTextEditor));

        // ToolbarBorderBrush property
        public static readonly DependencyProperty ToolbarBorderBrushProperty =
            DependencyProperty.Register("ToolbarBorderBrush", typeof(Brush),
            typeof(RichTextEditor));

        // ToolbarBorderThickness property
        public static readonly DependencyProperty ToolbarBorderThicknessProperty =
            DependencyProperty.Register("ToolbarBorderThickness", typeof(Thickness),
            typeof(RichTextEditor));


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();
            this.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The CodeControlsVisibility dependency property.
        /// </summary>
        [Browsable(true)]
        [Category("Visibility")]
        [Description("Whether the code controls are visible in the toolbar.")]
        [DefaultValue("Collapsed")]
        public Visibility CodeControlsVisibility
        {
            get { return (Visibility)GetValue(CodeControlsVisibilityProperty); }
            set { SetValue(CodeControlsVisibilityProperty, value); }
        }

        /// <summary>
        /// The WPF FlowDocument contained in the control.
        /// </summary>
        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        /// <summary>
        /// The ToolbarBackground dependency property.
        /// </summary>
        [Browsable(true)]
        [Category("Brushes")]
        [Description("The background color of the formatting toolbar on the control.")]
        [DefaultValue("Gainsboro")]
        public Brush ToolbarBackground
        {
            get { return (Brush) GetValue(ToolbarBackgroundProperty); }
            set { SetValue(ToolbarBackgroundProperty, value); }
        }

        /// <summary>
        /// The ToolbarBorderBrush dependency property.
        /// </summary>
        [Browsable(true)]
        [Category("Brushes")]
        [Description("The color of the formatting toolbar border.")]
        [DefaultValue("Gray")]
        public Brush ToolbarBorderBrush
        {
            get { return (Brush) GetValue(ToolbarBorderBrushProperty); }
            set { SetValue(ToolbarBorderBrushProperty, value); }
        }

        /// <summary>
        /// The thickness of the formatting toolbar border.
        /// </summary>
        [Browsable(true)]
        [Category("Other")]
        [Description("The thickness of the formatting toolbar border.")]
        [DefaultValue("1,1,1,0")]
        public Thickness ToolbarBorderThickness
        {
            get { return (Thickness) GetValue(ToolbarBorderThicknessProperty); }
            set { SetValue(ToolbarBorderThicknessProperty, value); }
        }

        #endregion

        #region PropertyChanged Callback Methods

        /// <summary>
        /// Called when the Document property is changed
        /// </summary>
        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /* For unknown reasons, this method gets called twice when the 
             * Document property is set. Until we figure out why, we initialize
             * the flag to 2 and decrement it each time through this method. */

            // Initialize
            var thisControl = (RichTextEditor)d;

            // Exit if this update was internally generated
            if (thisControl.m_InternalUpdatePending > 0)
            {

                // Decrement flags and exit
                thisControl.m_InternalUpdatePending--;
                return;
            }

            // Set Document property on RichTextBox
            thisControl.TextBox.Document = (e.NewValue == null) ? new FlowDocument() : (FlowDocument)e.NewValue;

            // Reset flag
            thisControl.m_TextHasChanged = false;
        } 

        #endregion

        #region Event Handlers

        /// <summary>
        /// Implements single-select on the alignment button group.
        /// </summary>
        private void OnAlignmentButtonClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = (ToggleButton)sender;
            var buttonGroup = new[] { LeftButton, CenterButton, RightButton, JustifyButton };
            this.SetButtonGroupSelection(clickedButton, m_SelectedAlignmentButton, buttonGroup, true);
            m_SelectedAlignmentButton = clickedButton;
        }

        /// <summary>
        /// Formats code blocks.
        /// </summary>
        private void OnCodeBlockClick(object sender, RoutedEventArgs e)
        {
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, "Consolas");
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, "FireBrick");
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 11D);
            textRange.ApplyPropertyValue(Block.MarginProperty, new Thickness(0));
        }

        /// <summary>
        /// Changes the font family of selected text.
        /// </summary>
        private void OnFontFamilyComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyCombo.SelectedItem == null) return;
            var fontFamily = FontFamilyCombo.SelectedItem.ToString();
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
        }

        /// <summary>
        /// Changes the font size of selected text.
        /// </summary>
        private void OnFontSizeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Exit if no selection
            if (FontSizeCombo.SelectedItem == null) return;

            // clear selection if value unset
            if (FontSizeCombo.SelectedItem.ToString() == "{DependencyProperty.UnsetValue}")
            {
                FontSizeCombo.SelectedItem = null;
                return;
            }

            // Process selection
            var pointSize = FontSizeCombo.SelectedItem.ToString();
            var pixelSize = Convert.ToDouble(pointSize) * (96 / 72);
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, pixelSize);
        }

        /// <summary>
        /// Formats inline code.
        /// </summary>
        private void OnInlineCodeClick(object sender, RoutedEventArgs e)
        {
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, "Consolas");
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 11D);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, "FireBrick");
        }

        /// <summary>
        /// Implements single-select on the alignment button group.
        /// </summary>
        private void OnListButtonClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = (ToggleButton)sender;
            var buttonGroup = new[] { BulletsButton, NumberingButton };
            this.SetButtonGroupSelection(clickedButton, m_SelectedListButton, buttonGroup, false);
            m_SelectedListButton = clickedButton;
        }

        /// <summary>
        /// Formats regular text
        /// </summary>
        private void OnNormalTextClick(object sender, RoutedEventArgs e)
        {
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamily);
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, FontSize);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Foreground);
            textRange.ApplyPropertyValue(Block.MarginProperty, new Thickness(Double.NaN));
        }

        /// <summary>
        /// Updates the toolbar when the text selection changes.
        /// </summary>
        private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            this.SetToolbar();
        }

        /// <summary>
        ///  Invoked when the user changes text in this user control.
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Set the TextChanged flag
            m_TextHasChanged = true; 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Forces an update of the Document property.
        /// </summary>
        public void UpdateDocumentBindings()
        {
            // Exit if text hasn't changed
            if (!m_TextHasChanged) return;

            // Set 'Internal Update Pending' flag
            m_InternalUpdatePending = 2;

            // Set Document property
            SetValue(DocumentProperty, this.TextBox.Document); 
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the control.
        /// </summary>
        private void Initialize()
        {
            FontFamilyCombo.ItemsSource = Fonts.SystemFontFamilies;
            FontSizeCombo.Items.Add("10");
            FontSizeCombo.Items.Add("12");
            FontSizeCombo.Items.Add("14");
            FontSizeCombo.Items.Add("18");
            FontSizeCombo.Items.Add("24");
            FontSizeCombo.Items.Add("36");
        }

        /// <summary>
        /// Sets a selection in a button group.
        /// </summary>
        /// <param name="clickedButton">The button that was clicked.</param>
        /// <param name="currentSelectedButton">The currently-selected button in the group.</param>
        /// <param name="buttonGroup">The button group to which the button belongs.</param>
        /// <param name="ignoreClickWhenSelected">Whether to ignore a click on the button when it is selected.</param>
        private void SetButtonGroupSelection(ToggleButton clickedButton, ToggleButton currentSelectedButton, IEnumerable<ToggleButton> buttonGroup, bool ignoreClickWhenSelected)
        {
            /* In some cases, if the user clicks the currently-selected button, we want to ignore
             * the click; for example, when a text alignment button is clicked. In other cases, we
             * want to deselect the button, but do nothing else; for example, when a list butteting
             * or numbering button is clicked. The ignoreClickWhenSelected variable controls that
             * behavior. */

            // Exit if currently-selected button is clicked
            if (clickedButton == currentSelectedButton)
            {
                if(ignoreClickWhenSelected) clickedButton.IsChecked = true;
                return;
            }

            // Deselect all buttons
            foreach (var button in buttonGroup)
            {
                button.IsChecked = false;
            }

            // Select the clicked button
            clickedButton.IsChecked = true;
        }

        /// <summary>
        /// Sets the toolbar.
        /// </summary>
        private void SetToolbar()
        {
            // Set font family combo
            var textRange = new TextRange(TextBox.Selection.Start, TextBox.Selection.End);
            var fontFamily = textRange.GetPropertyValue(TextElement.FontFamilyProperty);
            FontFamilyCombo.SelectedItem = fontFamily;

            // Set font size combo
            var fontSize = textRange.GetPropertyValue(TextElement.FontSizeProperty);
            FontSizeCombo.Text = fontSize.ToString();

            // Set Font buttons
            if (!String.IsNullOrEmpty(textRange.Text))
            {
                BoldButton.IsChecked = textRange.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold);
                ItalicButton.IsChecked = textRange.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic);
                UnderlineButton.IsChecked = textRange.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline);
            }

            // Set Alignment buttons
            LeftButton.IsChecked = textRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).Equals(TextAlignment.Left);
            CenterButton.IsChecked = textRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).Equals(TextAlignment.Center);
            RightButton.IsChecked = textRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).Equals(TextAlignment.Right);
            JustifyButton.IsChecked = textRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).Equals(TextAlignment.Justify);
        }

        #endregion
    }
}
