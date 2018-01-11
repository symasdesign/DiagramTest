using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace OrgChartControllerExample {
    public class RibbonHelper {
        public static void ToggleRibbonPageGroupItems(RibbonPageGroup pageGroup, bool enableButtons) {
            foreach (BarItemLink link in pageGroup.ItemLinks) {
                // you can filter links and items here
                link.Item.Enabled = enableButtons;
                BarButtonGroup group = link.Item as BarButtonGroup;
                if (group != null) ToggleRibbonButtonGroupItems(group, enableButtons);
            }
        }
        static void ToggleRibbonButtonGroupItems(BarButtonGroup buttonGroup, bool enableButtons) {
            foreach (BarItemLink subLink in buttonGroup.ItemLinks)
                subLink.Item.Enabled = enableButtons;
        }
    }
}
