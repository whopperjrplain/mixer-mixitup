﻿using MixItUp.Base;
using MixItUp.Base.ViewModel.User;
using System.Linq;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for UserDialogControl.xaml
    /// </summary>
    public partial class UserDialogControl : UserControl
    {
        private UserViewModel user;

        public UserDialogControl(UserViewModel user)
        {
            this.user = user;

            InitializeComponent();

            this.Loaded += UserDialogControl_Loaded;
        }

        private async void UserDialogControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await this.user.RefreshDetails();

            this.UserAvatar.SetSize(100);
            this.UserAvatar.SetImageUrl(this.user.AvatarLink);

            if (this.user.MixerRoles.Contains(MixerRoleEnum.Banned))
            {
                this.UnbanButton.Visibility = System.Windows.Visibility.Visible;
                this.BanButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.DataContext = this.user;
        }
    }
}
