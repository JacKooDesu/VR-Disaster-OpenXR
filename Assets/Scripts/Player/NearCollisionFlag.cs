using UnityEngine;

public class NearCollisionFlag : MonoBehaviour
{
    public enum EType
    {
        None,
        Left,
        Right,
    }
    [field: SerializeField]
    public EType Type { get; private set; }
}

public static class NearCollisionFlagExtension
{
    public static bool ReadAsHandInteractingState(this NearCollisionFlag self)
    {
        if (self.Type is NearCollisionFlag.EType.None)
            return false;

        var p = GameHandler.Singleton.player;
        if (p is null)
            return false;

        return self.Type switch
        {
            NearCollisionFlag.EType.Left => p.State_LInteracting,
            NearCollisionFlag.EType.Right => p.State_RInteracting,
            _ => false
        };
    }

    public static bool ReadAsHandGripState(this NearCollisionFlag self)
    {
        if (self.Type is NearCollisionFlag.EType.None)
            return false;

        var p = GameHandler.Singleton.player;
        if (p is null)
            return false;

        return self.Type switch
        {
            NearCollisionFlag.EType.Left => p.State_LGrip,
            NearCollisionFlag.EType.Right => p.State_RGrip,
            _ => false
        };
    }

    public static float ReadAsHandTriggerState(this NearCollisionFlag self)
    {
        if (self.Type is NearCollisionFlag.EType.None)
            return 0f;

        var p = GameHandler.Singleton.player;
        if (p is null)
            return 0f;

        return self.Type switch
        {
            NearCollisionFlag.EType.Left => p.State_LTrigger,
            NearCollisionFlag.EType.Right => p.State_RTrigger,
            _ => 0f
        };
    }
}
