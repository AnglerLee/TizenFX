using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.WonUI.Normal
{
    public class TabItemBuilder
    {
        private readonly TabItem _item = new TabItem();

        public TabItemBuilder SetText(string text)
        {
            _item.TabText.Text = text;
            return this;
        }

        public TabItemBuilder SetIcon(string resourcePath)
        {
            _item.TabIcon.ResourceUrl = resourcePath;
            return this;
        }

        public TabItemBuilder SetBadge(int count)
        {
            if (count > 0)
            {
                _item.Badge.Show();
                // 배지에 텍스트 추가
                var badgeText = new TextLabel
                {
                    Text = count.ToString(),
                    TextColor = Color.White,
                    PointSize = 8,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _item.Badge.Add(badgeText);
            }
            return this;
        }

        public TabItem Build() => _item;
    }


}