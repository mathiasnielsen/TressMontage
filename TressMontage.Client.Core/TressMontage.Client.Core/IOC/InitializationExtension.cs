using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace TressMontage.Core.IOC
{
    public class InitializationExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Context.Strategies.Add(new InitializableStrategy(), UnityBuildStage.PostInitialization);
        }
    }
}