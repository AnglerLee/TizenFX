namespace Tizen.WonUI
{
    public interface INavigationItem
    {
        string Id { get; }
        string Label { get; set; }
        string ResourceUrl { get; set; }
        bool IsSelected { get; set; }
        IBadge Badge { get; }
        void OnSelected();
    }
}