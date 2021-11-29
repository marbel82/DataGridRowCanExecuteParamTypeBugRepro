using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataGridRowCanExecuteParamTypeBugRepro
{
    public class Dog
    {
        public string Name { get; init; }
    }

    public class Cow
    {
        public string Identity { get; init; }
    }

    public class DogsCatalog
    {
        public string Title { get; } = "Dogs catalog";
        public ObservableCollection<Dog> Items { get; } = new ObservableCollection<Dog>();

        // I want DelegateCommand<Dog> here, but this causes an exception, when CanRemoveExecute is called
        public DelegateCommand<object> RemoveCommand { get; }

        public DogsCatalog()
        {
            RemoveCommand = new DelegateCommand<object>(RemoveExecute, CanRemoveExecute);
        }

        private void RemoveExecute(object item)
        {
            Items.Remove((Dog)item);
        }

        private bool CanRemoveExecute(object item)
        {
            return item is null or Dog; // Try breakpoint here, and look at `this` and `item` types
        }
    }

    public class CowsCatalog
    {
        public string Title { get; } = "Cows catalog";
        public ObservableCollection<Cow> Items { get; } = new ObservableCollection<Cow>();

        // I want DelegateCommand<Cow> here, but this causes an exception when CanRemoveExecute is called
        public DelegateCommand<object> RemoveCommand { get; }

        public CowsCatalog()
        {
            RemoveCommand = new DelegateCommand<object>(RemoveExecute, CanRemoveExecute);
        }

        private void RemoveExecute(object item)
        {
            Items.Remove((Cow)item);
        }

        private bool CanRemoveExecute(object item)
        {
            return item is null or Cow; // Try breakpoint here, and look at `this` and `item` types
        }
    }

    /// <summary>
    /// Interaction logic for DelayedDataContextSwitching.xaml
    /// </summary>
    public partial class DelayedDataContextSwitching : Window
    {
        public DogsCatalog DogsCatalog = new();
        public CowsCatalog CowsCatalog = new();

        public DelayedDataContextSwitching()
        {
            InitializeComponent();

            DataContext = DogsCatalog;

        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == DogsCatalog)
                DataContext = CowsCatalog;
            else
                DataContext = DogsCatalog;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == DogsCatalog)
                DogsCatalog.Items.Add(new Dog { Name = "Hektor" });
            else
                CowsCatalog.Items.Add(new Cow { Identity = "524632" });
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == DogsCatalog)
            {
                if (DogsCatalog.Items.Count > 0)
                    DogsCatalog.Items.RemoveAt(0);
            }
            else
            {
                if (CowsCatalog.Items.Count > 0)
                    CowsCatalog.Items.RemoveAt(0);
            }
        }

        private void GCButton_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
