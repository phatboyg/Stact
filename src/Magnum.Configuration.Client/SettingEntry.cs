namespace Magnum.Configuration.Client
{
    public class SettingEntry :
        ISetting
    {
        public int Id { get; set; }
        public string Application { get; set; }
        public string Environment { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
