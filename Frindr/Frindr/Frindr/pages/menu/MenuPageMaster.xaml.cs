using Frindr.pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPageMaster : ContentPage
    {
        public ListView ListView;

        public MenuPageMaster()
        {
            InitializeComponent();

            BindingContext = new MenuPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MenuPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuPageMenuItem> MenuItems { get; set; }
            
            public MenuPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MenuPageMenuItem>(new[]
                {
                    
                    new MenuPageMenuItem { Id = 0, Title = "Vrienden Zoeker", TargetType = typeof(FriendFinderPage) },
                    new MenuPageMenuItem { Id = 1, Title = "Profiel Pagina", TargetType = typeof(ProfilePage)},
                    new MenuPageMenuItem { Id = 2, Title = "Uitloggen", TargetType = typeof(LogoutPage)},
                    new MenuPageMenuItem { Id = 3, Title = "Afsluiten", TargetType = typeof(ExitPage)},
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}