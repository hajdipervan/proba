namespace dumpnovo.Classes
{
    public class Osoba
    {
        public string PersonName { get; set; }
        public string PersonSurname { get; }
        public string Email { get; }
        private Dictionary<Guid, bool> Presence { get; set; }
        public Osoba(string surname, string email)
        {
            PersonName = "";
            PersonSurname = surname;
            Email = email;
            Presence = new Dictionary<Guid, bool>();
        }
        public void ChangePresence(List<string> mails, Guid id)
        {
            if (mails.Contains(Email))
                Presence.Add(id, true);
        }
        public List<Event> Events(List<Event> events)
        {
            List<Event> personEvents = new List<Event>();
            foreach (var ev in events)
                if (Presence.ContainsKey(ev.Id)) personEvents.Add(ev);
            return personEvents;
        }
        public void RemovePresence(Guid id)
        {
            Presence[id] = false;
        }
        public string CheckHasEvent(Guid id)
        {
            if (Presence.ContainsKey(id) == false) { return ""; }
            if (Presence[id] == true) {return "prisutan"; }
            return "nije prisutan";
        }
        public bool CheckFree(Guid id)
        {
            if (Presence.ContainsKey(id)==false || Presence[id] == false) { return true; }
            return false; 
        }
    }
}
