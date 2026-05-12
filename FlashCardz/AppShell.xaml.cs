namespace FlashCardz;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("LoginPage",    typeof(Pages.LoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(Pages.RegisterPage));
        Routing.RegisterRoute("HomePage",     typeof(Pages.HomePage));
        Routing.RegisterRoute("LearnPage",    typeof(Pages.LearnPage));
        Routing.RegisterRoute("EditDeckPage", typeof(Pages.EditPage));
        Routing.RegisterRoute("ProfilePage",  typeof(Pages.ProfilePage));
        Routing.RegisterRoute("CreateDeckPage", typeof(Pages.CreateDeckPage));
    }
}