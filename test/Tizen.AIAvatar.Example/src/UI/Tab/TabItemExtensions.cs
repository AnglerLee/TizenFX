namespace Tizen.NUI2.Components.Reference
{
    public static class TabItemExtension
    {
        public static TView Spacing<TView>(this TView view, float value) where TView : TabItem
        {
            view.Spacing = value;
            return view;
        }
    }
}
