using Microsoft.Practices.Unity;

namespace TressMontage.Client.Extensions
{
    public static class UnityExtensions
    {
        public static IUnityContainer RegisterSingleton<TInterface, TType>(this IUnityContainer container) where TType : TInterface
        {
            return container.RegisterType<TInterface, TType>(new ContainerControlledLifetimeManager());
        }
    }
}
