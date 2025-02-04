using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.WonUI
{
    public class MaterialIndicator : View
    {
        private Animation _expandAnimation;
        private Animation _shrinkAnimation;

        public MaterialIndicator()
        {
            BackgroundColor = new Color(0.13f, 0.59f, 0.95f, 1.0f); // Material primary color
            Scale = new Vector3(0, 1, 0);
            Opacity = 1.0f;

            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            var interpolation = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine);
            
            // Expand animation
            _expandAnimation = new Animation(300);
            
            _expandAnimation.AnimateTo(this, "Scale", new Vector3(1, 1, 0), interpolation);
            _expandAnimation.AnimateTo(this, "Opacity", 1.0f, interpolation);

           
 
            // Shrink animation
            _shrinkAnimation = new Animation(200);
            _shrinkAnimation.AnimateTo(this, "Scale",  new Vector3(0, 1, 0), interpolation);
            _shrinkAnimation.AnimateTo(this, "Opacity", 0.0f, interpolation);
        }

        public new void Show()
        {
            _expandAnimation.Play();
        }

        public new void Hide()
        {
            _shrinkAnimation.Play();
        }
    }

    // State Ripple Effect
    public class RippleEffect : View
    {
        private Animation _rippleAnimation;

        public RippleEffect()
        {
            BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.1f);
            CornerRadius = 20;
            Scale = new Vector3(0, 0, 0);
            Opacity = 0.0f;

            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            var interpolation = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOut);

            _rippleAnimation = new Animation(400);
            _rippleAnimation.AnimateTo(this, "Scale", new Vector3(1, 1, 1), interpolation);
            _rippleAnimation.AnimateTo(this, "Opacity", 0.0f, interpolation);

            _rippleAnimation.Finished += (s, e) => Scale = new Vector3(0, 0, 0);
        }

        public void PlayAt(Position touchPosition)
        {
            Position = touchPosition;
            Scale = new Vector3(1, 1, 1);
            Opacity = 1.0f;
            _rippleAnimation.Play();
        }
    }

}