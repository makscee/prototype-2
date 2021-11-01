using System;
using UnityEngine;

namespace UI
{
    public class MobileObjectEnabler : MonoBehaviour
    {
        [SerializeField] bool mobileEnable;
        void OnEnable()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            gameObject.SetActive(mobileEnable);
#else
            gameObject.SetActive(!mobileEnable);
#endif
        }
    }
}