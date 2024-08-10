using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace ListenTools.Resources.DataTemplates.Selector;

public class MessageContentTemplateSelector : IDataTemplate
{
    public Control? Build(object? param)
    {
        var key = param?.ToString();
        if (key is null) 
        {
            throw new ArgumentNullException(nameof(param));
        }
        //return App.Templ[key].Build(param); // finally we look up the provided key and let the System build the DataTemplate for us
        return null;
    }

    public bool Match(object? data)
    {
        var key = data?.ToString();

        // return data is ShapeType                      
        //        && !string.IsNullOrEmpty(key)           
        //        && AvailableTemplates.ContainsKey(key); 
        return true;
    }
}