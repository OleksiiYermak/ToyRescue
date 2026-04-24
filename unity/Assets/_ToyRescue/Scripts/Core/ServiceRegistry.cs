using System;
using System.Collections.Generic;

namespace ToyRescue.Core
{
    public sealed class ServiceRegistry
    {
        private readonly Dictionary<Type, object> servicesByType = new Dictionary<Type, object>();

        public static ServiceRegistry Current { get; private set; }

        public static void SetCurrent(ServiceRegistry registry)
        {
            Current = registry;
        }

        public void Register<TService>(TService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            servicesByType[typeof(TService)] = service;
        }

        public bool TryGet<TService>(out TService service)
        {
            if (servicesByType.TryGetValue(typeof(TService), out object value) && value is TService typedValue)
            {
                service = typedValue;
                return true;
            }

            service = default;
            return false;
        }

        public TService Get<TService>()
        {
            if (TryGet(out TService service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service not registered: {typeof(TService).Name}");
        }

        public void Clear()
        {
            servicesByType.Clear();
        }
    }
}
