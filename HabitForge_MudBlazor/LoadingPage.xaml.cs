using HabitForge_MudBlazor.Data;

namespace HabitForge_MudBlazor;

public partial class LoadingPage : ContentPage
{
	private readonly IServiceProvider _serviceProvider;
	public LoadingPage(IServiceProvider service, AppDbContext dbcontext)
	{
		InitializeComponent();
		_serviceProvider = service;
	}
	protected override async void OnAppearing(){
		base.OnAppearing();

		var dbTask = Task.Run(() =>
		{
			var db = _serviceProvider.GetRequiredService<AppDbContext>();
		});
		var mainPage = _serviceProvider.GetRequiredService<MainPage>();
		await Task.WhenAll(dbTask , Task.Delay(2000));
		MainThread.BeginInvokeOnMainThread(() =>
		{
			Application.Current.MainPage = mainPage;
		});
    }
}