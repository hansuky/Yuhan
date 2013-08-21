using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace NETKey
{
    /// <summary>
    /// Provides a means to specify DataTemplates to be selected from within WPF code
    /// </summary>
    public class DynamicTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Generic attached property specifying <see cref="Template"/>s used by the <see cref="DynamicTemplateSelector"/>
        /// </summary>
        /// <remarks>
        /// This attached property will allow you to set the templates you wish to be available whenever
        /// a control's TemplateSelector is set to an instance of <see cref="DynamicTemplateSelector"/>
        /// </remarks>
        public static readonly DependencyProperty TemplatesProperty = DependencyProperty.RegisterAttached("Templates", typeof(Template[]), typeof(DependencyObject));
        
        /// <summary>
        /// Sets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> for which the attached property's value will be set</param>
        /// <param name="value">The array of <see cref="Template"/> objects to be used by the given <paramref name="element"/></param>
        public static void SetTemplates(UIElement element, Template[] value)
        {
            element.SetValue(TemplatesProperty, value);
        }
        
        /// <summary>
        /// Gets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> who's attached template's property you wish to retrieve</param>
        /// <returns>The templates used by the givem <paramref name="element"/> when using the <see cref="DynamicTemplateSelector"/></returns>
        public static Template[] GetTemplates(UIElement element)
        {
            return (Template[])element.GetValue(TemplatesProperty);
        }

        /// <summary>
        /// Overriden base method to allow the selection of the correct DataTemplate
        /// </summary>
        /// <param name="item">The item for which the template should be retrieved</param>
        /// <param name="container">The object containing the current item</param>
        /// <returns>The <see cref="DataTemplate"/> to use when rendering the <paramref name="item"/></returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            //First, we gather all the templates associated with the current control through our dependency property
            Template[] templates = (Template[])container.GetValue(TemplatesProperty);

            //Then we go through them checking if any of them match our criteria
            foreach (var template in templates)
                //In this case, we are checking whether the type of the item is the same as the type supported by our DataTemplate
                if (template.Value.IsInstanceOfType(item))
                    //And if it is, then we return that DataTemplate
                    return template.DataTemplate;
            
            //If all else fails, then we go back to using the default DataTemplate
            return base.SelectTemplate(item, container);
        }
    }

    /// <summary>
    /// Provides a link between a value and a <see cref="DataTemplate"/>
    /// for the <see cref="DynamicTemplateSelector"/>
    /// </summary>
    /// <remarks>
    /// In this case, our value is a <see cref="System.Type"/> which we are attempting to match
    /// to a <see cref="DataTemplate"/>
    /// </remarks>
    public class Template : DependencyObject
    {
        /// <summary>
        /// Provides the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Type), typeof(Template));

        /// <summary>
        /// Provides the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(Template));

        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public Type Value
        { get { return (Type)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        /// <summary>
        /// Gets or Sets the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public DataTemplate DataTemplate
        { get { return (DataTemplate)GetValue(DataTemplateProperty); } set { SetValue(DataTemplateProperty, value); } }
    }
}
