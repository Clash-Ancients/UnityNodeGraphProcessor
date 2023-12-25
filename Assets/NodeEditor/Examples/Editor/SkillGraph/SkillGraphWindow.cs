using GraphProcessor;

namespace Plugins.NodeEditor
{
    
    public class SkillGraphWindow : UniversalGraphWindow
    {
        protected override void	InitializeWindow(BaseGraph graph){
            graphView = new NPBehaveGraphView(this);
        }
    }

}
