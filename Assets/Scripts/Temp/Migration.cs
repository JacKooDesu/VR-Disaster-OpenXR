using UnityEngine;
using UnityEngine.EventSystems;

public class XRInputManager
{
    public static class Instance
    {
        public static bool Button(object type, object device)
        {
            return true;
        }


        public static T GetInputData<T>(XRHandlerDeviceType type)
        {
            return default(T);
        }
    }

}

public class CustomControllerBehaviour
{
    public object Device { get; set; }
    public Transform transform { get; set; }
}

public class XRHandlerData
{
    public bool IsHandLostTracking { get; set; }
    public bool IsHandNotFound { get; set; }
}

public enum XRHandlerDeviceType
{
    LeftHand,
    RightHand
}

public class XRBaseRaycaster
{
    public Transform HitResult { get; }
}