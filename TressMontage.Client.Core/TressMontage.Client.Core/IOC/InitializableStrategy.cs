using Microsoft.Practices.ObjectBuilder2;

namespace TressMontage.Core.IOC
{
    public class InitializableStrategy : IBuilderStrategy
    {
        public void PreBuildUp(IBuilderContext context)
        {
        }

        public async void PostBuildUp(IBuilderContext context)
        {
            var initObj = context.Existing as IInitializable;
            if (initObj != null)
            {
                await initObj.Initialize();
            }
        }

        public void PreTearDown(IBuilderContext context)
        {
        }

        public void PostTearDown(IBuilderContext context)
        {
        }
    }
}
