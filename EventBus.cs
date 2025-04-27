using System.Collections.Generic;

namespace System.Events
{
    public sealed class EventBus
    {
        private readonly struct EventKey : IEquatable<EventKey>
        {
            public string EventName { get; }
            public Type EventType { get; }

            public EventKey(string name, Type type)
            {
                EventName = name;
                EventType = type;
            }

            public bool Equals(EventKey other)
            {
                return EventName == other.EventName && EventType == other.EventType;
            }
        }

        public static readonly EventBus Instance = new EventBus();
        private readonly Dictionary<EventKey, Delegate> _events = new Dictionary<EventKey, Delegate>();
        private readonly object _lock = new object();

        /// <summary>
        /// Subscribes a delegate to the specified event name.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate to subscribe.</typeparam>
        /// <param name="eventName">The unique name of the event.</param>
        /// <param name="callback">The delegate to invoke when the event is published.</param>
        /// <returns>True if subscription succeeds; otherwise, false.</returns>
        public bool Subscribe<TDelegate>(string eventName, TDelegate callback)
            where TDelegate : Delegate
        {
            if (string.IsNullOrEmpty(eventName) || callback == null)
            {
                return false;
            }

            var key = new EventKey(eventName, typeof(TDelegate));

            lock (_lock)
            {
                if (_events.TryGetValue(key, out var value))
                {
                    _events[key] = Delegate.Combine(value, callback);
                }
                else
                {
                    _events.Add(key, callback);
                }
            }
            return true;
        }

        /// <summary>
        /// Unsubscribes all delegates associated with the specified event name.
        /// </summary>
        /// <param name="eventName">The name of the event to unsubscribe.</param>
        /// <returns>True if unsubscription succeeds; otherwise, false.</returns>
        public bool Unsubscribe(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return false;
            }

            var keysToRemove = new List<EventKey>();

            lock (_lock)
            {
                foreach (var kvp in _events)
                {
                    if (kvp.Key.EventName == eventName)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _events.Remove(key);
                }
            }
            return true;
        }

        /// <summary>
        /// Unsubscribes a specific delegate from the specified event name.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate to unsubscribe.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>True if unsubscription succeeds; otherwise, false.</returns>
        public bool Unsubscribe<TDelegate>(string eventName, TDelegate callback)
            where TDelegate : Delegate
        {
            if (string.IsNullOrEmpty(eventName) || callback == null)
            {
                return false;
            }

            var key = new EventKey(eventName, typeof(TDelegate));

            lock (_lock)
            {
                if (_events.TryGetValue(key, out var value))
                {
                    _events[key] = Delegate.Remove(value, callback);
                }
            }
            return true;
        }

        /// <summary>
        /// Clears all subscribed events and delegates.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _events.Clear();
            }
        }

        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="eventName">The name of the event to publish.</param>
        /// <returns>True if the event is published successfully; otherwise, false.</returns>
        public bool Publish(string eventName)
        {
            var key = new EventKey(eventName, typeof(Action));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action act)
            {
                act.Invoke();
            }
            return true;
        }

        /// <inheritdoc cref="Publish(string)" />
        public bool Publish<T>(string eventName, T arg)
        {
            var key = new EventKey(eventName, typeof(Action<T>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T> act)
            {
                act.Invoke(arg);
            }
            return true;
        }

        /// <inheritdoc cref="Publish(string)" />
        public bool Publish<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2> act)
            {
                act.Invoke(arg1, arg2);
            }
            return true;
        }

        /// <inheritdoc cref="Publish(string)" />
        public bool Publish<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2, T3>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2, T3> act)
            {
                act.Invoke(arg1, arg2, arg3);
            }
            return true;
        }

        /// <inheritdoc cref="Publish(string)" />
        public bool Publish<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2, T3, T4>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2, T3, T4> act)
            {
                act.Invoke(arg1, arg2, arg3, arg4);
            }
            return true;
        }
    }
}