namespace MiApp.UI.Tests;

public class MainWindowViewModelTests
{
    private sealed class FakePage : ViewModelBase
    {
        public int LoadCount { get; private set; }

        public override Task LoadAsync()
        {
            LoadCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void Start_navega_a_la_primera_pagina_y_la_carga()
    {
        var page = new FakePage();
        var factory = Substitute.For<IPageFactory>();
        factory.Create(typeof(HomeViewModel)).Returns(page);
        var vm = new MainWindowViewModel(factory);

        vm.Start();

        Assert.Same(page, vm.CurrentPage);
        Assert.Equal(1, page.LoadCount);
    }

    [Fact]
    public void Seleccionar_otra_pagina_crea_su_ViewModel()
    {
        var factory = Substitute.For<IPageFactory>();
        factory.Create(Arg.Any<Type>()).Returns(_ => new FakePage());
        var vm = new MainWindowViewModel(factory);

        vm.SelectedPage = vm.Pages.Single(p => p.ViewModelType == typeof(TodosViewModel));

        factory.Received(1).Create(typeof(TodosViewModel));
    }
}
