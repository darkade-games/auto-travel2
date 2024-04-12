using Microsoft.Xna.Framework;

namespace AutoTravel2;

public class TravelLocation
{
    public string name;
    public Vector2 position;
    public string region;
    public int facingDirection;
    public bool favorite = false;

    public TravelLocation(string _name, Vector2 _position, string _region, int _facingDirection)
    {
        this.name = _name;
        this.position = _position;
        this.region = _region;
        this.facingDirection = _facingDirection;
    }
}
