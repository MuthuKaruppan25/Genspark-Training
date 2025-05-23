using FlyWeight.Interfaces;
namespace FlyWeight.Models;
class LocationMarker
{
    private readonly int _latitude;
    private readonly int _longitude;
    private readonly IMarker _markerIcon;

    public LocationMarker(int latitude, int longitude, IMarker markerIcon)
    {
        _latitude = latitude;
        _longitude = longitude;
        _markerIcon = markerIcon;
    }

    public void Draw()
    {
        _markerIcon.Draw(_latitude, _longitude);
    }
}