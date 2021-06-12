using System.IO;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;

namespace Shimakaze.Framework
{
    public static class ContentManagerExtension
    {
        #region Public Methods

        public static T Load<T>(this ContentManager @this, params string[] filePath)
        {
            return @this.Load<T>(Path.Combine(filePath));
        }

        public static Task<T> LoadAsync<T>(this ContentManager @this, params string[] filePath)
        {
            Task<T> task = new(() => @this.Load<T>(Path.Combine(filePath)));
            task.Start();
            return task;
        }

        #endregion Public Methods
    }
}