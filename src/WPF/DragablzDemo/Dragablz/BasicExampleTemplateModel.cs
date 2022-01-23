using Dragablz;

namespace DragablzDemo;

public class BasicExampleTemplateModel
{
    public BasicExampleTemplateModel(IInterTabClient interTabClient, object partition)
    {
        InterTabClient = interTabClient;
        Partition = partition;            
    }

    public IInterTabClient InterTabClient { get; }

    public object Partition { get; }
}