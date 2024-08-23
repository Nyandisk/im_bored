namespace im_bored.util{
    public class Registry<T>(string name)
    {
        public string Name { get; private set; } = name;
        private readonly Dictionary<string,T> _storage = [];

        public T Grab(string identifier){
            return _storage[identifier];
        }
        public void Register(string identifier, T value){
            Console.WriteLine($"reg_{Name} > registered {identifier}");
            _storage.Add(identifier,value);
        }
        public void Unregister(string identifier){
            Console.WriteLine($"reg_{Name} > unregistered {identifier}");
            _storage.Remove(identifier);
        }
    }
}