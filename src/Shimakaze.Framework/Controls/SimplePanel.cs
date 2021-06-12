using System.Collections.Generic;

namespace Shimakaze.Framework.Controls
{
    public class SimplePanel : PanelBase
    {
        public SimplePanel() : base()
        {
        }
        public SimplePanel(IEnumerable<Control> children) : base(children)
        {
        }
    }
}
