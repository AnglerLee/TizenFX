using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.WonUI
{
    public class NavigationItem : View, INavigationItem
    {
        private readonly TextLabel _label;
        private readonly ImageView _icon;

        private readonly View _iconContainer;
        private readonly View _itemContainer;

        private readonly MaterialIndicator _indicator;
        private readonly RippleEffect _rippleEffect;
        private bool _isSelected;
        private IBadge _badge;

        public string Id { get; }

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public string ResourceUrl
        {
            get => _icon.ResourceUrl;
            set => _icon.ResourceUrl = value;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateVisualState();
                OnSelected();
            }
        }

        public IBadge Badge => _badge;

        public NavigationItem(string id, string label, string resourceUrl)
        {
            Id = id;

            _itemContainer = new View()
            {
                Layout = new LinearLayout{
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = 8
                }
            };

             _iconContainer = new View();
            _icon = new ImageView
            {
                ResourceUrl = resourceUrl,
                Size = new Size(24, 24),
                CornerRadius = 0.5f,
            };
            _badge = new NumericBadge();


            _iconContainer.Add(_icon);
            if (_badge != null)
            {
                _iconContainer.Add(_badge as View);
            }

            _label = new TextLabel { Text = label };
            _indicator = new MaterialIndicator() 
            {
                Size = new Size(0, 2),
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.UseAssignedSize,
                ParentOrigin = Position.ParentOriginBottomLeft,
            };
            _rippleEffect = new RippleEffect()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent, 
            };

            InitializeInteractions();

            // Layout setup and styling code here
            _itemContainer.Add(_iconContainer);
            _itemContainer.Add(_label);
            
            // Stack order: Ripple -> Icon -> Badge -> Indicator
            Add(_rippleEffect);
            Add(_itemContainer);
            Add(_indicator);

            UpdateVisualState();

        }

        protected virtual void UpdateVisualState()
        {
            // Update visual appearance based on selection state
            _badge.UpdateLayout();
            if (_isSelected)
            {
                _indicator.Show();
                _label.TextColor = new Color(0.13f, 0.59f, 0.95f, 1.0f);
                _icon.Color = new Color(0.13f, 0.59f, 0.95f, 1.0f);
            }
            else
            {
                _indicator.Hide();
                _label.TextColor = new Color(0, 0, 0, 0.6f);
                _icon.Color = new Color(0, 0, 0, 0.6f);
            }
        }

        public virtual void OnSelected()
        {
            // Handle selection logic
            Log.Debug("NavItem", $"Selected {Id} : {_isSelected}");
        }

        private void InitializeInteractions()
        {
            // Touch events
            TouchEvent += OnTouchEvent;

            // Hover events (if platform supports)
            HoverEvent += OnHoverEvent;
        }

        private bool OnTouchEvent(object source, TouchEventArgs e)
        {
            switch (e.Touch.GetState(0))
            {
                case PointStateType.Down:
                    _rippleEffect.PlayAt(new Position(0,0));
                    return true;

                case PointStateType.Up:
                    /*if (ContainsPoint(e.Touch.GetLocalPosition(0)))
                    {
                        OnSelected();
                    }*/
                    return true;
            }

            return false;
        }

        private bool OnHoverEvent(object source, HoverEventArgs e)
        {
            
            return true;
        }

    }
}