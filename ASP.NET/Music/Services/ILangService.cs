using MusicPortal.Models;

namespace MusicPortal.Services
{
    public interface ILangRead
    {
        List<Language> languageList();
    }
    public class ReadLangServices : ILangRead
    {
        IConfiguration _con;
        List<Language> languageLists;
        public ReadLangServices(IConfiguration con)
        {
            string section = "Lang";
            _con = con;
            IConfigurationSection pointSection = _con.GetSection(section);
            List<Language> lists = new List<Language>();
            foreach (var language in pointSection.AsEnumerable())
            {
                if (language.Value != null)
                    lists.Add(new Language
                    {
                        ShortName = language.Key.Replace(section + ":", ""),
                        Name = language.Value
                    });
            }

            languageLists = lists;
        }

        public List<Language> languageList() => languageLists;
    }
}
