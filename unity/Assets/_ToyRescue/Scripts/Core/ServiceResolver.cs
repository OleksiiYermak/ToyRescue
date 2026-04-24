using UnityEngine;

namespace ToyRescue.Core
{
    public static class ServiceResolver
    {
        public static GameBootstrapper ResolveBootstrapper(ref GameBootstrapper bootstrapper)
        {
            if (bootstrapper == null)
            {
                bootstrapper = Object.FindFirstObjectByType<GameBootstrapper>();
            }

            return bootstrapper;
        }

        public static bool TryResolve<TService>(ref GameBootstrapper bootstrapper, out TService service)
        {
            ServiceRegistry registry = ResolveRegistry(ref bootstrapper);
            if (registry != null && registry.TryGet(out service))
            {
                return true;
            }

            service = default;
            return false;
        }

        public static bool TryResolve<TServiceA, TServiceB>(
            ref GameBootstrapper bootstrapper,
            out TServiceA serviceA,
            out TServiceB serviceB)
        {
            ServiceRegistry registry = ResolveRegistry(ref bootstrapper);
            if (registry == null)
            {
                serviceA = default;
                serviceB = default;
                return false;
            }

            bool hasServiceA = registry.TryGet(out serviceA);
            bool hasServiceB = registry.TryGet(out serviceB);
            return hasServiceA && hasServiceB;
        }

        public static ServiceRegistry ResolveRegistry(ref GameBootstrapper bootstrapper)
        {
            if (ServiceRegistry.Current != null)
            {
                return ServiceRegistry.Current;
            }

            ResolveBootstrapper(ref bootstrapper);
            return ServiceRegistry.Current ?? bootstrapper?.Services;
        }
    }
}
