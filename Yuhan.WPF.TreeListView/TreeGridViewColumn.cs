using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Yuhan.WPF
{
    public class TreeGridViewColumn : GridViewColumn
    {
        public Boolean Expandable
        {
            get { return (Boolean)GetValue(ExpandableProperty); }
            set { SetValue(ExpandableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Expandable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandableProperty =
            DependencyProperty.Register("Expandable", typeof(Boolean), typeof(TreeGridViewColumn), new PropertyMetadata(false, ExpandableChanged));

        private static void ExpandableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as TreeGridViewColumn).Expander.SetBinding(ToggleButton.VisibilityProperty, new Binding()
            {
                Source = e.NewValue,
                Converter = new BooleanToVisibilityConverter()
            });
        }

        public String FieldName
        {
            get { return (String)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register("FieldName",
            typeof(String), typeof(TreeGridViewColumn),
            new PropertyMetadata(null, OnFieldNameChanged));

        protected static void OnFieldNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((obj) as TreeGridViewColumn).ContentControlFactory.SetBinding(ContentControl.ContentProperty, new Binding(e.NewValue.ToString()));
        }



        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TreeGridViewColumn), new PropertyMetadata(new DataTemplate(), ContentTemplateChanged));

        private static void ContentTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((obj) as TreeGridViewColumn).ContentControlFactory.SetValue(ContentControl.ContentTemplateProperty, e.NewValue);
        }

        private FrameworkElementFactory expander;

        public FrameworkElementFactory Expander
        {
            get { return expander; }
            set { expander = value; }
        }


        private FrameworkElementFactory contentControlFactory;

        public FrameworkElementFactory ContentControlFactory
        {
            get { return contentControlFactory; }
            set { contentControlFactory = value; }
        }


        public TreeGridViewColumn()
            : base()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/Yuhan.WPF.TreeListView;component/Resources/TreeListView.xaml");

            //this.CellTemplate = resourceDictionary["CellTemplate"] as DataTemplate;
            
            DataTemplate template = new DataTemplate();

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(DockPanel));

            Expander = new FrameworkElementFactory(typeof(ToggleButton));
            Expander.Name = "Expander";
            Expander.SetValue(ToggleButton.StyleProperty, resourceDictionary["ExpandCollapseToggleStyle"] as Style);
            Expander.SetBinding(ToggleButton.VisibilityProperty, new Binding()
            {
                Source = this.Expandable,
                Converter = new BooleanToVisibilityConverter()
            });
            Expander.SetBinding(ToggleButton.MarginProperty, new Binding("Level")
            {
                Converter = new LevelToIndentConverter(),
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1)
            });
            Expander.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsExpanded")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1)
            });
            Expander.SetValue(ToggleButton.ClickModeProperty, ClickMode.Press);

            ContentControlFactory = new FrameworkElementFactory(typeof(ContentControl));
            ContentControlFactory.SetBinding(ContentControl.ContentProperty, new Binding(FieldName));

            factory.AppendChild(Expander);
            factory.AppendChild(ContentControlFactory);

            template.VisualTree = factory;
            template.Triggers.Add(new DataTrigger()
            {
                Binding = new Binding("HasItems")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1),
                },
                Value = false,
                Setters = { new Setter(ToggleButton.VisibilityProperty, Visibility.Hidden, "Expander") }
            });
            this.CellTemplate = template;
        }
    }
}
