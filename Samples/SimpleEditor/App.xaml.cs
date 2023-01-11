using System.Collections.Generic;
using System.Windows;
using Clarity;
using SimpleEditor.Main;

namespace SimpleEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new DefaultBootstrapper();

            var vm = ServiceManager.Default.Resolve<MainViewModel>();
            vm.People.AddRange(GetPeople());

            ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, false, 400, 400);
        }

        private IEnumerable<Domain.Person> GetPeople()
        {
            var list = new List<Domain.Person>();
            list.Add(CreatePerson("Fred", "Flintstone"));
            list.Add(CreatePerson("Wilma", "Flintstone"));
            list.Add(CreatePerson("Pebbles", "Flintstone"));

            list.Add(CreatePerson("Barney", "Rubble"));
            list.Add(CreatePerson("Betty", "Rubble"));
            list.Add(CreatePerson("Bamm-Bamm", "Rubble"));

            return list;
        }

        private Domain.Person CreatePerson(string firstName, string lastName)
        {
            var p = new Domain.Person();
            p.FirstName = firstName;
            p.LastName = lastName;

            return p;
        }
    }
}
