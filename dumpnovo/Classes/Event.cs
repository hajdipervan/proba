namespace dumpnovo.Classes
{
    public class Event
    {
        public Guid Id { get; } 
        public string EventName { get; set; }
        public string Location { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        private List<String> Mails { get;  set; }

        public Event(List<string> mails)
        {
            Id = Guid.NewGuid();
            EventName = "";
            Location = "";
            Mails = mails;
        }
        public List<string> ReturnMails()
        {
            return Mails;
        }
        public string ReturnId()
        {
            return Id.ToString();
        }
        public List<string> ChangeEventMails(List<string> maillist,List<Osoba> osobe)
        {
            List<string> mailPersonNotInEvent = new List<string>();
            foreach (var mail in maillist)
            {
                if (Mails.Contains(mail) == false) { mailPersonNotInEvent.Add(mail); }
                Mails.Remove(mail);
                foreach(var os in osobe)
                    if (os.Email == mail)
                        os.RemovePresence(Id);
            }
            return mailPersonNotInEvent;
        }
    }
}
