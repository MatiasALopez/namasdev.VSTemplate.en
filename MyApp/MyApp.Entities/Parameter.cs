namespace MyApp.Entities
{
    public partial class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
