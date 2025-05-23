using FlyWeight.Models;
using FlyWeight.Interfaces;
namespace FlyWeight.Services;

class MarkerFactory
{
    private readonly Dictionary<string, MarkerIcon> _markers = new Dictionary<string, MarkerIcon>();

    
    private string GetKey(string iconType, string color, string style)
    {
        return $"{iconType}_{color}_{style}";
    }

    public MarkerIcon GetMarker(string iconType, string color, string style)
    {
        string key = GetKey(iconType, color, style);

        if (!_markers.ContainsKey(key))
        {
            _markers[key] = new MarkerIcon(iconType, color, style);
            Console.WriteLine($"Creating new marker icon: {key}");
        }
        else
        {
            Console.WriteLine($"Reusing existing marker icon: {key}");
        }

        return _markers[key];
    }
}