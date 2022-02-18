using QRMeteo.DBExcel;
using QRMeteo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
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
            //SetDataToView();

            inventoryList.ItemsSource = Model.inventory;
        }

        public void SetDataToView()
        {
            Label header = new Label
            {
                Text = "Локальная база",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

        ListView listView = new ListView
            {
            HasUnevenRows = true,
            // Определяем источник данных
            ItemsSource = Model.inventory,

            // Определяем формат отображения данных
            ItemTemplate = new DataTemplate(() =>
            {
                    // привязка к свойству Name
                    Label titleLabel = new Label { FontSize = 18 };
                titleLabel.SetBinding(Label.TextProperty, "Name");

                    // привязка к свойству Company
                    Label companyLabel = new Label();
                companyLabel.SetBinding(Label.TextProperty, "InventoryNumber");

                    // привязка к свойству Price
                    Label priceLabel = new Label();
                priceLabel.SetBinding(Label.TextProperty, "LocationItem");

                    // создаем объект ViewCell.
                    return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(0, 5),
                        Orientation = StackOrientation.Vertical,
                        Children = { titleLabel, companyLabel, priceLabel }
                    }
                };
            })
        };
            this.Content = new StackLayout { Children = { header, listView } };
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
            if (result)
            {
                Model.DeleteFile();
                ResultEntry.Text = "Excel очищен";
            }
        }

        public async void OnItemTappedAsync(object sender, ItemTappedEventArgs e)
        {
            IntventoryObject invObj = e.Item as IntventoryObject;
            string uri = invObj.TargetHttpPosString;

            try
            {
                if (uri.Length > 0)
                {
                    string url = HttpUtility.UrlDecode(uri);//конвертим в url
                    // Так как ссылаемся на ячейки из гугл таблиц необходимо наличие программы Google Табицы и надичие доступа к таблице
                    await Browser.OpenAsync(new Uri(uri), BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}