namespace im_bored.util
{
    public class Registry<T>(string name) where T : IDisposable
    {
        public string Name { get; private set; } = name;
        private readonly Dictionary<string, T> _storage = [];

        public T Grab(string identifier)
        {
            return _storage[identifier];
        }
        public void Register(string identifier, T value)
        {
            if (_storage.ContainsKey(identifier))
            {
                Console.WriteLine($"reg_{Name} > {identifier} is already registered");
                return;
            }
            Console.WriteLine($"reg_{Name} > registered {identifier}");
            _storage.Add(identifier, value);
        }
        public void Unregister(string identifier)
        {
            if (!_storage.ContainsKey(identifier))
            {
                Console.WriteLine($"reg_{Name} > {identifier} does not exist, cannot remove");
                return;
            }
            Console.WriteLine($"reg_{Name} > unregistered {identifier}");
            _storage.Remove(identifier);
        }
        public void FreeRegistry()
        {
            Console.WriteLine($"reg_{Name} > disposing");
            foreach (T disposable in _storage.Values)
            {
                disposable.Dispose();
            }
            _storage.Clear();
            GC.Collect();
        }
    }
}