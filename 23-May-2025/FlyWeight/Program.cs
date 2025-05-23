using System;
using FlyWeight.Models;
using FlyWeight.Interfaces;
using FlyWeight.Services;
class Program
{
    static void Main()
    {
        MarkerFactory factory = new MarkerFactory();

        List<LocationMarker> mapMarkers = new List<LocationMarker>();


        mapMarkers.Add(new LocationMarker(10, 20, factory.GetMarker("Pin", "Red", "Solid")));
        mapMarkers.Add(new LocationMarker(15, 25, factory.GetMarker("Pin", "Red", "Solid")));  // reuse
        mapMarkers.Add(new LocationMarker(30, 45, factory.GetMarker("Pin", "Blue", "Dashed")));
        mapMarkers.Add(new LocationMarker(35, 50, factory.GetMarker("Circle", "Green", "Dotted")));
        mapMarkers.Add(new LocationMarker(40, 55, factory.GetMarker("Pin", "Red", "Solid")));  // reuse

        
        foreach (var marker in mapMarkers)
        {
            marker.Draw();
        }
    }
}