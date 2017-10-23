﻿using Client.DataAccess.Model;
using Client.DataAccess.Repository;
using Client.Desktop.Helper;
using Client.Desktop.View;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Desktop.ViewModel
{
    public class CashFlowsPageViewModel : BaseViewModel
    {
        private ObservableCollection<CashFlow> _cashFlows;

        private CashFlow _cashFlow;

        private CashFlowCategory _cashFlowCategory;

        private ObservableCollection<CashFlowCategory> _cashFlowCategories;

        public ICommand RunAddDialogCommand => new RelayCommand(ExecuteRunAddDialog);

        public ICommand RunAddCategoryDialogCommand => new RelayCommand(ExecuteRunAddCategoryDialog);

        public ICommand RunUpdateDialogCommand => new RelayCommand(ExecuteRunUpdateDialog);

        public ICommand RunDeleteCategoryDialogCommand => new RelayCommand(ExecuteRunDeleteCategoryDialog);

        public ICommand RunDeleteDialogCommand => new RelayCommand(ExecuteRunDeleteDialog);

        public AddCategoryDialogViewModel addCategoryDialogViewModel;

        public AddCashFlowDialogViewModel addCashFlowDialogViewModel;

        public UpdateCashFlowDialogViewModel updateCashFlowDialogViewModel;

        public DeleteCategoryDialogViewModel deleteCategoryDialogViewModel; 

        public DeleteCashFlowDialogViewModel deleteCashFlowDialogViewModel;

        
        public CashFlowsPageViewModel()
        {
            CashFlowsRepository CashFlowsRepository = new CashFlowsRepository();
            var allCashFlows = CashFlowsRepository.GetAllCashFlows(Properties.Login.Default.AccountId);
            CashFlows = new ObservableCollection<CashFlow>(allCashFlows);
            if (CashFlows.Any())
                SelectedCashFlow = CashFlows[0];

            CashFlowCategories = new ObservableCollection<CashFlowCategory>(CashFlowCategoryRepository.GetAllParentCashFlowCategories());
        }

        public ObservableCollection<CashFlow> CashFlows
        {
            get
            {
                return _cashFlows;
            }
            set
            {
                this.MutateVerbose(ref _cashFlows, value, RaisePropertyChanged());
            }
        }

        public CashFlow SelectedCashFlow
        {
            get
            {
                return _cashFlow;
            }
            set
            {
                this.MutateVerbose(ref _cashFlow, value, RaisePropertyChanged());
            }
        }

        public ObservableCollection<CashFlowCategory> CashFlowCategories
        {
            get
            {
                return _cashFlowCategories;
            }
            set
            {
                this.MutateVerbose(ref _cashFlowCategories, value, RaisePropertyChanged());
            }
        }

        public CashFlowCategory SelectedCategory
        {
            get
            {
                return _cashFlowCategory;
            }
            set
            {
                this.MutateVerbose(ref _cashFlowCategory, value, RaisePropertyChanged());
            }
        }

        private async void ExecuteRunAddCategoryDialog(object o)
        {
            //Chack if there is selected category
            addCategoryDialogViewModel = new AddCategoryDialogViewModel();
            var view = new AddCategoryDialog
            {
                DataContext = addCategoryDialogViewModel
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingAddCategoryDialogEventHandler);

            //check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }
        private void ClosingAddCategoryDialogEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == true)
            {
                CashFlowCategoryRepository.AddCategory(Properties.Login.Default.AccountId, addCategoryDialogViewModel.SelectedParentCashFlowCategory?.Id, addCategoryDialogViewModel.CashFlowCategory);
                CashFlowCategories = new ObservableCollection<CashFlowCategory>(CashFlowCategoryRepository.GetAllParentCashFlowCategories());
            }
        }

        private async void ExecuteRunAddDialog(object o)
        {
            addCashFlowDialogViewModel = new AddCashFlowDialogViewModel();
            var view = new AddCashFlowDialog
            {

                DataContext = addCashFlowDialogViewModel
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingAddDialogEventHandler);

            //check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ClosingAddDialogEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == true)
            {
                //TODO: add check
                CashFlowsRepository CashFlowsRepository = new CashFlowsRepository();
                var cashAccountSelectedItem = addCashFlowDialogViewModel.SelectedCashAccount as CashAccount;
                var categorySelectedItem = addCashFlowDialogViewModel.SelectedCashFlowCategory as CashFlowCategory;

                CashFlowsRepository.AddCashFlow(cashAccountSelectedItem.Id, addCashFlowDialogViewModel.Amount, categorySelectedItem.Id, Convert.ToDateTime(addCashFlowDialogViewModel.Date), addCashFlowDialogViewModel.Description);
                //Renue content DataGrid
                CashFlows = new ObservableCollection<CashFlow>(CashFlowsRepository.GetAllCashFlows(Properties.Login.Default.AccountId));
                //Set to zero fields
                addCashFlowDialogViewModel = new AddCashFlowDialogViewModel();
            }
        }

        private async void ExecuteRunUpdateDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            updateCashFlowDialogViewModel = new UpdateCashFlowDialogViewModel(SelectedCashFlow);
            var view = new UpdateCashFlowDialog
            {

                DataContext = updateCashFlowDialogViewModel
            };


            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingUpdateDialogEventHandler);

            //check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ClosingUpdateDialogEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == true)
            {
                CashFlowsRepository CashFlowsRepository = new CashFlowsRepository();
                var cashAccountSelectedItem = updateCashFlowDialogViewModel.SelectedCashAccount as CashAccount;
                var categorySelectedItem = updateCashFlowDialogViewModel.SelectedCashFlowCategory as CashFlowCategory;

                CashFlowsRepository.UpdateCashFlow(SelectedCashFlow.Id, cashAccountSelectedItem.Id, updateCashFlowDialogViewModel.Amount, categorySelectedItem.Id, Convert.ToDateTime(updateCashFlowDialogViewModel.Date), updateCashFlowDialogViewModel.Description);
                //Renue content DataGrid
                CashFlows = new ObservableCollection<CashFlow>(CashFlowsRepository.GetAllCashFlows(Properties.Login.Default.AccountId));
            }
        }

        private async void ExecuteRunDeleteCategoryDialog(object o)
        {
            deleteCategoryDialogViewModel = new DeleteCategoryDialogViewModel(SelectedCategory);
            var view = new DeleteCategoryDialog
            {
                DataContext = deleteCategoryDialogViewModel
            };
            
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingDeleteCategoryDialogEventHandler);
        }

        private void ClosingDeleteCategoryDialogEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == true)
            {
                CashFlowCategoryRepository.DeleteCategory(SelectedCategory.Id);
                //Renue content Categories TreeView
                CashFlowCategories = new ObservableCollection<CashFlowCategory>(CashFlowCategoryRepository.GetAllParentCashFlowCategories());
            }
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            deleteCashFlowDialogViewModel = new DeleteCashFlowDialogViewModel(SelectedCashFlow);
            var view = new DeleteCashFlowDialog
            {
                DataContext = deleteCashFlowDialogViewModel
            };
            
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingDeleteDialogEventHandler);
        }

        private void ClosingDeleteDialogEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == true)
            {
                CashFlowsRepository CashFlowsRepository = new CashFlowsRepository();
                CashFlowsRepository.DeleteCashFlow(SelectedCashFlow.Id);
                //Renue content DataGrid
                CashFlows = new ObservableCollection<CashFlow>(CashFlowsRepository.GetAllCashFlows(Properties.Login.Default.AccountId));
            }
        }

    }
}
