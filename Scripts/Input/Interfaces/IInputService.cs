using UniRx;
using UnityEngine;

namespace Input.Interfaces
{
    public interface IInputService
    {
        Subject<ControllerType> DeviceChanged { get; } 
        Subject<Vector2> MoveVectorChanged { get; }
    }
}
