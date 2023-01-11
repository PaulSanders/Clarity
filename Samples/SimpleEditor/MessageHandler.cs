using Clarity;

namespace SimpleEditor
{
    class MessageHandler : Disposable
    {
        public MessageHandler()
        {
            MessageBus.Subscribe<EditPersonMessage>(OnReceive);
        }

        private void OnReceive(EditPersonMessage msg)
        {
            var vm = ServiceManager.Default.Resolve<Person.PersonViewModel>();

            vm.Person = msg.Person;
            ServiceManager.Default.Resolve<IWindowManager>().ShowDialog(vm, true, true, false);
        }
    }
}
