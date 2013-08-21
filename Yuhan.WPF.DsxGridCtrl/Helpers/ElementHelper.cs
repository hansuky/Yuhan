using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Yuhan.WPF.DsxGridCtrl
{
    public static class ElementHelper
    {
        #region Method - FindVisualChild

        public static T FindVisualChild<T>(DependencyObject parent, string childName = "")
            where T : DependencyObject
        {      
            if (parent == null) return null;  

            T       _foundChild     = null;  
            int     _childrenCount  = VisualTreeHelper.GetChildrenCount(parent);  

            for (int i = 0; i < _childrenCount; i++)  
            {    
                if (_foundChild != null) 
                {
                    break;
                }

                DependencyObject _child = VisualTreeHelper.GetChild(parent, i);

                T   _childType = _child as T;    
                if (_childType == null)    
                {      
                    _foundChild = FindVisualChild<T>(_child, childName);
                }
                else if (!string.IsNullOrEmpty(childName))    
                {      
                    FrameworkElement _frameworkElement = _child as FrameworkElement;

                    if (_frameworkElement != null && _frameworkElement.Name == childName)
                    {        
                        _foundChild = (T)_child;
                    }    
                    else
                    {
                        _foundChild = FindVisualChild<T>(_child, childName);
                    }
                }
                else
                {
                    _foundChild = (T)_child;
                }  
            }  
            return _foundChild;
        }
        #endregion

        #region Method - FindLogicalParent

        public static T FindLogicalParent<T>(DependencyObject child, string parentName = "")
            where T : DependencyObject
        {      
            if (child == null) return null;  

            T _foundParent     = null;  

            DependencyObject _parent = LogicalTreeHelper.GetParent(child);

            if (_parent == null)
            {
                FrameworkElement _frameworkElement = child as FrameworkElement;
                if (_frameworkElement != null && _frameworkElement.TemplatedParent != null)
                {
                    _parent = _frameworkElement.TemplatedParent;
                }
            }

            T   _parentType = _parent as T;
            if (_parentType == null)    
            {      
                _foundParent = FindLogicalParent<T>(_parent);
            }
            else if (!string.IsNullOrEmpty(parentName))    
            {      
                FrameworkElement _frameworkElement = _parent as FrameworkElement;

                if (_frameworkElement != null && _frameworkElement.Name == parentName)
                {        
                    _foundParent = (T)_parent;
                }    
                else
                {
                    _foundParent = FindLogicalParent<T>(_parent);
                }
            }
            else
            {
                _foundParent = (T)_parent;
            }  
            return _foundParent;
        }
        #endregion
    }
}
