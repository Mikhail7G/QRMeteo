using QRMeteo.DBExcel;
using QRMeteo.ViewModels;
using System;
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
            this.BindingContext = Model;
        }

        private void InventoryRefreshing(object sender, EventArgs e) //обновление данных в View
        {
            inventoryList.ItemsSource = null;
            inventoryList.ItemsSource = Model.Inventory;
            inventoryList.IsRefreshing = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            inventoryList.BeginRefresh();
        }

        private async void BackBUttonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ClearLocalDBBtn(object sender,EventArgs e)
        {
            bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить базу данных?", "Да", "Нет");
            if (result)
            {
                Model.ClearWiewList();
            }
        }

        private void WriteToExelButtonClicked(object sender, EventArgs e)
        {
            SetTextResultLabel("Запись в файл");
        }

        private async void DeleteExelFileButtonClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить фаил EXEL?", "Да", "Нет");
            if (result)
            {
                Model.DeleteFile();
                SetTextResultLabel("Excel очищен");
            }
        }

        /// <summary>
        /// Открывает новое окно с детальным описанием выбранного объекта
        /// </summary>
        public async void OnItemTappedAsync(object sender, ItemTappedEventArgs e)
        {
            InventoryObject invObj = e.Item as InventoryObject;
            try
            {
                ItemEditorPage page = new ItemEditorPage();
                await Navigation.PushAsync(page);
                page.SetItemObject(invObj);

                ((ListView)sender).SelectedItem = null;// очищаем выделение из листа
            }
            catch (Exception ex)
            {

            }
        }     
        
        private void SetTextResultLabel(string text)
        {
            if (text.Length > 0)
            {
                ResultEntry.Text = text;
            }
        }
    }
}