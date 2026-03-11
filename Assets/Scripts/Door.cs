using UnityEngine;

public enum DoorDirection
{
    Left,
    Right,
    Up,
    Down
}

public class Door : MonoBehaviour
{
    public DoorDirection direction;
    public bool used = false;
}