namespace Tizen.NUI2.Components.Reference
{
    /// <summary>
    /// Provides extension methods for a style of <see cref="TabItem"/> type.
    /// </summary>
    public static class TabItemStyleExtension
    {
        /// <summary>
        /// Adds <see cref="StyleProperty{TTabItem, TValue}"> that sets <see cref="TabItem.Spacing"> with given value.
        /// </summary>
        /// <typeparam name="TTabItem">The stylable view implementing <see cref="ITabItemContent"/>.</typeparam>
        /// <param name="style">The style to add the property to.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>The style itself.</returns>
        public static Style<TTabItem> Spacing<TTabItem>(this Style<TTabItem> style, float value) where TTabItem : TabItem
        {
            style.Add(new StyleProperty<TTabItem, float>(nameof(Spacing), value, (c, v) => c.Spacing(v)));
            return style;
        }
    }
}
