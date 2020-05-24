namespace Fisk.EnterpriseManageViewModel
{
    public class User
    {
        public string userid { get; set; }
        public string username { get; set; }
        public int departID { get; set; }
        public string Position { get; set; }

        public bool OnlyCheck { get; set; } = false;

        public bool Isprincipal { get; set; } = false;

        public string EmailAddress { get; set; }
    }
}
