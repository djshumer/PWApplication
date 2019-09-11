using System;
using System.Globalization;
using System.Reflection;
using Autofac;
using PWApplication.MobileShared.Services.Dependency;
using PWApplication.MobileShared.Services.DialogService;
using PWApplication.MobileShared.Services.Identity;
using PWApplication.MobileShared.Services.NavigationService;
using PWApplication.MobileShared.Services.OpenUrl;
using PWApplication.MobileShared.Services.RequestProvider;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.Services.Transactions;
using PWApplication.MobileShared.Services.UserInfo;
using PWApplication.MobileShared.Services.Users;
using Xamarin.Forms;

namespace PWApplication.MobileShared.ViewModels.Base
{
    public class ViewModelLocator
    {
        private static IContainer _container;

        public static bool UseMockService { get; set; } = false;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        static ViewModelLocator()
        {
            UpdateDependencies(UseMockService);
        }

        public static void UpdateDependencies(bool useMockServices)
        {
            var builder = new ContainerBuilder();

            // View models

            builder.RegisterType<Services.Dependency.DependencyService>().As<IDependencyService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RequestProvider>().As<IRequestProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DialogService>().As<IDialogService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingsService>().As<ISettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<OpenUrlService>().As<IOpenUrlService>().InstancePerLifetimeScope();
            builder.RegisterType<IdentityService>().As<IIdentityService>().InstancePerLifetimeScope();
            builder.RegisterType<NavigationService>().As<INavigationService>().InstancePerLifetimeScope();
            

            if (useMockServices)
            {
                builder.RegisterType<UserMockService>().As<IUserService>().InstancePerLifetimeScope();
                builder.RegisterType<MockTransactionService>().As<ITransactionService>().InstancePerLifetimeScope();
                builder.RegisterType<MockUserInfoService>().As<IUserInfoService>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<TransactionService>().As<ITransactionService>().InstancePerLifetimeScope();
                builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
                builder.RegisterType<UserInfoService>().As<IUserInfoService>().InstancePerLifetimeScope();
            }


            //View Models
            builder.RegisterType<AboutViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<LoginViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<MainViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryTransactionsViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<NewTransactionViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionDetailViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<SearchUserViewModel>().InstancePerLifetimeScope();


            UseMockService = useMockServices; 

            _container = builder.Build();

        }


        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public MainViewModel MainViewModel => Resolve<MainViewModel>();

        public LoginViewModel LoginViewModel => Resolve<LoginViewModel>();

        public AboutViewModel AboutViewModel => Resolve<AboutViewModel>();

        public ProfileViewModel ProfileViewModel => Resolve<ProfileViewModel>();

        public HistoryTransactionsViewModel HystoryTransactionsViewModel => Resolve<HistoryTransactionsViewModel>();



        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }

        
    }
}
