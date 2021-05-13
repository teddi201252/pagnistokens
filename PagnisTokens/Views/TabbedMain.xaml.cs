using FormsControls.Base;
using MySqlConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedMain : TabbedPage, IAnimationPage
    {
        public TabbedMain()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public IPageAnimation PageAnimation { get; } = new FadePageAnimation { Duration = AnimationDuration.Short, Subtype = AnimationSubtype.Default };

        public void OnAnimationFinished(bool isPopAnimation) { }

        public void OnAnimationStarted(bool isPopAnimation) { }
    }
}
