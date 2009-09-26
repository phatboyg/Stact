namespace Magnum.Configuration.Client
{
    using Data;
    using System.Linq;

    public class SettingsManager :
        IReadWriteSettings
    {
        private IRepository _repository;
        private string _environment;
        private string _application;

        public SettingsManager(IRepository repository, string environment, string application)
        {
            _repository = repository;
            _environment = environment;
            _application = application;
        }

        public ISetting Get(string key)
        {
            //query by the environment id
            //then by the application id
            //lastly by the configuration key
            var result = _repository.FindBy<SettingEntry>().Where(o=>o.Key==key).First();
            return result;
        }

        public void Put(string key, string value)
        {
            //if it exists update 

            //else insert
            var setting = new SettingEntry()
                          {
                              Application = _application,
                              Environment = _environment,
                              Id = 0,
                              Key = key,
                              Value = value
                          };

            _repository.Save(setting);
        }
    }
}