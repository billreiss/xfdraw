using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDrawSamples
{
    class NavigationService
    {
        NavigationPage navPage;
        private static NavigationService instance;

        internal static NavigationService Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("NavigationService.Init must be called before using");
                }
                return instance;
            }
        }

        public static void Init(NavigationPage page)
        {
            instance = new NavigationService(page);
        }

        protected NavigationService(NavigationPage page)
        {
            this.navPage = page;
        }

        public async Task NavigateHomeAsync(bool animated = true)
        {
            await navPage.PopToRootAsync(animated);
        }

        public async Task NavigateToAsync(Type pageType, bool animated = true, params object[] args)
        {
            object page;

            var pInfo = pageType.GetTypeInfo();

            var xfPage = typeof(Xamarin.Forms.Page).GetTypeInfo();

            if (pInfo.IsAssignableFrom(xfPage) || pInfo.IsSubclassOf(typeof(Xamarin.Forms.Page)))
            {
                page = Activator.CreateInstance(pageType, args);
            }
            else
            {
                throw new ArgumentException("Page Type must be based on Xamarin.Forms.Page");
            }
            await navPage.PushAsync(page as Xamarin.Forms.Page);
        }
    }
}
