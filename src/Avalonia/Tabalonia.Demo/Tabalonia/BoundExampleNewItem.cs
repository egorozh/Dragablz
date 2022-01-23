using System;
using Dragablz;

namespace Tabalonia.Demo;

public static class BoundExampleNewItem
{
    public static Func<HeaderedItemViewModel> Factory 
    {
        get
        {
            return
                () =>
                {
                    var dateTime = DateTime.Now;

                    return new HeaderedItemViewModel()
                    {
                        Header = dateTime.ToLongTimeString(),
                        Content = dateTime.ToString("R")
                    };
                };
        }        
    }
}