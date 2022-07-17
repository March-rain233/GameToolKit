using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    public abstract class EventBase
    {
    }
    public abstract class GenericEvent<T> : EventBase
    {
        public T Value;
    }
    public class NormalEvent : EventBase
    {

    }
}
