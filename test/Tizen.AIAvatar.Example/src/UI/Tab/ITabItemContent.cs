namespace Tizen.NUI2.Components.Reference
{
    /// <summary>
    /// TabItem content interface.
    /// TabItem creates body with this interface.
    /// </summary>
    public interface ITabItemContent : IComponentContent, ITextElement, IImage
    {
        float Spacing { get; set; }
    }
}
