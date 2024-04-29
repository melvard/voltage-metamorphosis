using Schemes.Dashboard;
using UnityEngine;

namespace Schemes.Device.Wire
{
    public class WireNode : MonoBehaviour
    {
        private DashboardGridElement _dashboardPathNode;
        public DashboardGridElement PathNode => _dashboardPathNode;

        public void SetPathNode(DashboardGridElement node)
        {
            _dashboardPathNode = node;
        }
    }
}