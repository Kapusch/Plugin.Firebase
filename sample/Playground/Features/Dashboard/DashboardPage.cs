using Playground.Common.Base;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Vm = Playground.Features.Dashboard.DashboardViewModel;

namespace Playground.Features.Dashboard
{
    public sealed class DashboardPage : ContentPageBase<Vm>
    {
        public DashboardPage() => Build();

        private void Build()
        {
            InitializeViewModel();
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            BackgroundColor = Color.White;

            Content = new StackLayout {
                Visual = VisualMarker.Material,
                Margin = 32,
                Children = {
                    new Button { Text = "Remote Config", TextColor = Color.Black }
                        .Bind(nameof(Vm.NavigateToRemoteConfigPageCommand)),
                    new Button { Text = "Storage", TextColor = Color.Black }
                        .Bind(nameof(Vm.NavigateToStoragePageCommand))
                }
            }
                .FillHorizontal()
                .CenterVertical();
        }
    }
}