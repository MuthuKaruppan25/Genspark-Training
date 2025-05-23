using FlyWeight.Interfaces;
namespace FlyWeight.Models;
class MarkerIcon : IMarker
{
    private readonly string _iconType;  
    private readonly string _color;    
    private readonly string _style;     

    public MarkerIcon(string iconType, string color, string style)
    {
        _iconType = iconType;
        _color = color;
        _style = style;
    }

    public void Draw(int latitude, int longitude)
    {
        Console.WriteLine($"Drawing '{_color} {_style} {_iconType}' marker at [{latitude}, {longitude}]");
    }

   
}