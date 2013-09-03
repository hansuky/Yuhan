using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Yuhan.WPF.MenuKiller
{
    /// <summary>
    /// NOTE:
    /// The MenuKillerItem 
    /// IS a ICustomAlignedControl, and due to its template, it also
    /// HAS an ICustomAlignedControl as its only child, which will, in fact, do the measure/arrange for us.
    /// </summary>

    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Panel", Type = typeof(CircularPanel))]
    [TemplatePart(Name = "PART_AlignPanel", Type = typeof(ReferenceAlignPanel))]
    public class MenuKillerItem : TreeViewItem, ICommandSource, ICustomAlignedControl
    {
        static MenuKillerItem()    
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuKillerItem),
                new FrameworkPropertyMetadata(typeof(MenuKillerItem)));

            CommandBinding binding = new CommandBinding(MenuKillerCommands.ToggleExpansion);
            binding.Executed += new ExecutedRoutedEventHandler(ToggleCommandHandler);
            binding.CanExecute += new CanExecuteRoutedEventHandler(CanToggleHandler);

            CommandManager.RegisterClassCommandBinding(typeof(MenuKillerItem), binding);
        }

        public MenuKillerItem()
        {
        }

        #region Private Members
        private Button mCenterButton;
        private CircularPanel mPanel;
        private ReferenceAlignPanel mAlignPanel;
        #endregion

        #region Attached Event MouseHover
        public static readonly RoutedEvent MouseHoverEvent =
            EventManager.RegisterRoutedEvent("MouseHover",
                                            RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(MenuKillerItem));

        public static void AddMouseHoverHandler(DependencyObject o, RoutedEventHandler handler)
        {
            ((UIElement)o).AddHandler(MenuKillerItem.MouseHoverEvent, handler);
        }

        public static void RemoveMouseHoverHandler(DependencyObject o, RoutedEventHandler handler)
        {
            ((UIElement)o).RemoveHandler(MenuKillerItem.MouseHoverEvent, handler);
        }
        #endregion

        #region ICommandSource Dependency Properties
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(MenuKillerItem),
                new PropertyMetadata((ICommand)null,
                new PropertyChangedCallback(CommandChanged)));

        [Description("Assigns a Command to this MenuKillerItem executed upon button click unless this item has children.")]
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register(
                "CommandTarget",
                typeof(IInputElement),
                typeof(MenuKillerItem),
                new PropertyMetadata((IInputElement)null));

        [Description("Assigns a CommandTarget to this MenuKillerItem to apply the Command to.")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(MenuKillerItem),
                new PropertyMetadata((object)null));

        [Description("Assigns a CommandParameter to this MenuKillerItem which will be passed when executing Command.")]
        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty RootToolTipProperty =
            DependencyProperty.Register(
                "RootToolTip",
                typeof(object),
                typeof(MenuKillerItem),
                new PropertyMetadata(null));

        [Description("Used to retrieve the RootToolTip of the MenuKillerItem which is currently or was most recently under the mouse.")]
        public object RootToolTip
        {
            get
            {
                return (object)GetValue(RootToolTipProperty);
            }
            set
            {
                SetValue(RootToolTipProperty, value);
            }
        }

        #region CommandChanges
        private static void CommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            MenuKillerItem mki = (MenuKillerItem)d;
            mki.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }
 
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }

            AddCommand(oldCommand, newCommand);
        }

        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = new EventHandler(CanExecuteChanged);
            canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        private static EventHandler canExecuteChangedHandler;




        
        #endregion

        void MenuKillerItem_MouseEnter(object o, MouseEventArgs e)
        {
            Button sender = o as Button;

            if (null != sender)
            {
                sender.RaiseEvent(new RoutedEventArgs(MenuKillerItem.MouseHoverEvent, this)); 
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        
            // NOTE: Do not use GetTemplateChild(), because it will only find direct descendants.
            // Also refer to the MSDN Comment of GetTemplateChild() (shown in IntelliSense).
            // This is a common mistake.
            //
            // NOTE: FindName() also exist for this (from FrameworkElement), but we want to find
            // on the _template_
            mCenterButton = Template.FindName("PART_Button", this) as Button;

            if (null != mCenterButton)
            {
                mCenterButton.Click += new System.Windows.RoutedEventHandler(mCenterButton_Click);
                mCenterButton.MouseEnter += new MouseEventHandler(MenuKillerItem_MouseEnter);
            }

            mPanel = Template.FindName("PART_Panel", this) as CircularPanel;

            mAlignPanel = Template.FindName("PART_AlignPanel", this) as ReferenceAlignPanel;

            if (null != mPanel)
            {
                mPanel.ChildArranged += new CircularPanel.OnChildArranged(mPanel_ChildArranged);
            }
        }

        void mPanel_ChildArranged(object sender, UIElement child, double angle)
        {
            MenuKillerItem childItem = child as MenuKillerItem;

            childItem.mPanel.AngleSpacing = Double.NaN;

            // Good if only one submenu is open at a time
            // childItem.mPanel.StartAngle = angle - 67.75;
            // childItem.mPanel.EndAngle = angle + 67.75;

            // Good if multiple submenus are open at a time
            childItem.mPanel.StartAngle = angle - 45.0;
            childItem.mPanel.EndAngle = angle + 45.00;
        }

        /// <summary>
        /// Multiplies the opacity of all children with a given value, except for the center button and the 'exception'
        /// 
        /// </summary>
        /// <param name="exception">The item whose opacity is to be preserved. may be null.</param>
        /// <param name="multiplier">The value to mulitply the opacity with.</param>
        public void SetChildrenOpacity(UIElement exception, double dOpacity)
        {
            foreach(UIElement elemRover in mPanel.Children)
            {
                if (elemRover != exception)
                {
                    elemRover.Opacity = dOpacity;
                }
            }
        }

        void SetOpacityRecursively(int iLevel)
        {
            // 2.0 seemed a little too much. One might want this to be configureable
            double dTargetOp = 1.0 / Math.Pow(1.4, iLevel);

            if (Parent is MenuKillerItem)
            {
                ((MenuKillerItem)Parent).SetChildrenOpacity(this, dTargetOp);
                ((MenuKillerItem)Parent).SetOpacityRecursively(++iLevel);
            }
            
            mCenterButton.Opacity = dTargetOp;
        }

        void mCenterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mPanel.Children.Count > 0)
            {
                MenuKillerCommands.ToggleExpansion.Execute(null, this);
            }
            else
            {
                if (Command != null)
                {
                    RoutedCommand command = Command as RoutedCommand;

                    if (command != null)
                    {
                        command.Execute(CommandParameter, CommandTarget);
                    }
                    else
                    {
                        ((ICommand)Command).Execute(CommandParameter);
                    }
                }
            }
        }

        public Point AlignReferencePoint
        {
            get
            {
                if (null != mAlignPanel)
                {
                    return mAlignPanel.AlignReferencePoint;
                }
                return new Point();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size infSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

            mCenterButton.Measure(infSize);

            mAlignPanel.AlignReferencePoint = new Point(mCenterButton.DesiredSize.Width * 0.5, mCenterButton.DesiredSize.Height * 0.5);

            mPanel.Measure(infSize);
            mAlignPanel.Measure(infSize);

            return base.MeasureOverride(constraint);
        }
        
        // Can't use Conditional on overrides
#if DEBUG
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(null, new Pen(Brushes.DarkBlue, 3.0), new Rect(0, 0, DesiredSize.Width - Margin.Left - Margin.Right, DesiredSize.Height - Margin.Top - Margin.Bottom));
            // show the visual center
            drawingContext.DrawEllipse(Brushes.Red, null, AlignReferencePoint, 3, 3);
        }
#endif
 
        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            // TODO: If a given child element has already expanded children, we don't want to dial down the opacity even further!

            // Make sure we have the correct options for PART_Panel
            if (null != mPanel)
            {
                // The really really nice way is to check neighbour arragements and clip in a way such that we don't draw over it
                mPanel.Radius = Math.Max(mCenterButton.DesiredSize.Height, mCenterButton.DesiredSize.Width) * 0.75; 
            }

            if (this.IsExpanded)
            {
                SetOpacityRecursively(1);
            }
            else
            {
                SetOpacityRecursively(0);
            }

            InvalidateTreeMeasure();
        }

        void InvalidateTreeMeasure()
        {
            InvalidateMeasure();   
            mPanel.InvalidateMeasure();
            mAlignPanel.InvalidateMeasure();
            
            if (Parent is MenuKillerItem)
            {
                ((MenuKillerItem)Parent).InvalidateTreeMeasure();
            }
        }

        private bool CanToggleExpand()
        {
            if (null != Items && Items.Count > 0)
            {
                return true;
            }

            return false;
        }
        
        // Executed event handler.
        private static void ToggleCommandHandler(object target, ExecutedRoutedEventArgs e)
        {
            if (null != target && target is MenuKillerItem)
            {
                ((MenuKillerItem)target).ToggleExpand();
            }
            
        }

        // CanExecute event handler.
        private static void CanToggleHandler(object target, CanExecuteRoutedEventArgs e)
        {
            if (null != target && target is MenuKillerItem)
            {
                e.CanExecute = ((MenuKillerItem)target).CanToggleExpand();
            }
        } 
    }
}
