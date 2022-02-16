using QRMeteo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRMeteo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalDBPage : ContentPage
    {
        private ExportingViewModel Model;
        public LocalDBPage()
        {
            InitializeComponent();
        }

        public void SetViewModel(ExportingViewModel model)
        {
            Model = model;
            BindingContext = Model;
        }
        private async void BackBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void WriteToExelBtn_Clicked(object sender, EventArgs e)
        {
            ResultEntry.Text = "Запись в файл";
        }

        private async void DeleteExelBtn_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
            if(result)
            Model.DeleteFile();
        }
    }
}