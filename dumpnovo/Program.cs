using dumpnovo.Classes;

List<Osoba> osobe=UploadFirst10People();
List<Event> events = UploadFirst5Events(osobe);
string choice = "*";
while (choice == "*")
{
    choice = Answer("\n1 - Aktivni eventi \n2 - Nadolazeći eventi \n3 - Eventi koji su zavrsili \n4 - Kreiraj event \n5 - Izađi iz programa");
    if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5") { choice = Back("Neispravan unos", true); }
    if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice!="*") { System.Environment.Exit(0); }
    if (choice != "1" && choice != "2" && choice != "3" && choice != "*")
    {
        Console.Clear();
        string nameOfNewEvent = Answer("Unesite naziv eventa");
        string locationOfNewEvent = Answer("Unesite lokaciju eventa");
        Console.WriteLine("Unesite pocetak eventa(datum)");
        DateTime dateStartOfNewEvent, dateEndOfNewEvent;
        DateTime.TryParse(Console.ReadLine(), out dateStartOfNewEvent);
        Console.WriteLine("Unesite kraj eventa(datum)");
        DateTime.TryParse(Console.ReadLine(), out dateEndOfNewEvent);
        if (dateStartOfNewEvent < DateTime.Now || dateEndOfNewEvent < dateStartOfNewEvent) { choice = Back("Neispravan unos", false); }
        if (choice != "*")
        {
            if (Answer("Jeste li sigurni da zelite kreirati dogadaj? y/n")!="y") { choice = Back("Odustali ste od kreiranja eventa", false); }
            if (choice != "*")
            {
                string mailsForNewEvent = Answer("Unesite emailove pozvanih osoba");
                List<string> mailList = ReturnMail(mailsForNewEvent);
                List<Osoba> invitedPeople = ReturnInvitedPeople(osobe, mailList);
                List<Osoba> invitedPeopleForRemoving = new List<Osoba>();
                mailList.Clear();
                foreach (var os in invitedPeople)
                    foreach (var ev in os.Events(events))
                        if (ev.DateEnd < dateStartOfNewEvent || dateEndOfNewEvent < ev.DateStart || (dateStartOfNewEvent < ev.DateEnd && os.CheckFree(ev.Id) == true))
                            mailList.Add(os.Email);
                        else
                        {
                            invitedPeopleForRemoving.Add(os);
                            break;
                        }
                foreach (var os in invitedPeopleForRemoving)
                    invitedPeople.Remove(os);
                events.Add(CreateEvent(nameOfNewEvent, locationOfNewEvent, dateStartOfNewEvent, dateEndOfNewEvent, mailList, invitedPeople));
                choice = Back("Event je uspjesno kreiran", false);
                Console.WriteLine($"Naziv: {nameOfNewEvent}, Lokacija: {locationOfNewEvent}, Pocetak:{dateStartOfNewEvent}, Kraj: {dateEndOfNewEvent}");
            }
        }
    }
    if (choice != "1" && choice != "2" && choice != "*")
    {
        Console.Clear();
        choice=FinishedEventsChosen(events,osobe);
    }
    if (choice != "1" && choice != "*")
    {
        Console.Clear();
        foreach (var ev in events)
        {
            if (ev.DateStart > DateTime.Now)
            {
                Console.WriteLine($"\nNadolazeci event: \nId eventa: {ev.Id} \nNaziv eventa: {ev.EventName}, Lokacija: {ev.Location}, " +
                        $"Starts in: {decimal.Round((decimal)(ev.DateStart - DateTime.Now).TotalDays)} dana,  " +
                        $"Trajanje: {decimal.Round((decimal)(ev.DateEnd - ev.DateStart).TotalHours, 1)} sata \nPopis sudionika");
                DisplayParticipants(ev, osobe);
            }
        }
        string choice2 = Answer("1 - Izbrisi event \n2 - Ukloni osobe s eventa \n3 - Povratak na glavni menu");
        if (choice2 != "1" && choice2 != "2" && choice2 != "3") { choice = Back("Neispravan unos", true); }
        if (choice2 != "1" && choice2 != "2") { choice = Back("", true); }
        if (choice2 != "1" && choice!="*")
        {
            string idInsert = Answer("Unesi id eventa kojem zelis urediti sudionike");
            if (ListIdStrings(events).Contains(idInsert) == false) { choice = Back("Ne postoji dogadaj s tim ID-om", true); };
            if (choice != "*")
            {
                List<string> mailpeopleNotInEvent = new List<string>();
                string removingPeopleMails = Answer("Unesi mailove osoba koje zelis ukloniti s eventa");
                if (Answer("Jesi li siguran da navedene osobe zelis ukloniti s eventa? y/n") != "y") { choice = Back("Odustali ste od uredivanja", true); }
                if (choice != "*")
                {
                    foreach (var ev in events)
                        if (ev.Id.ToString() == idInsert)
                            mailpeopleNotInEvent = ev.ChangeEventMails(ReturnMail(removingPeopleMails), osobe);
                    foreach (var mail in mailpeopleNotInEvent)
                        Console.WriteLine($"{ReturnPerson(mail, osobe).PersonName} {ReturnPerson(mail, osobe).PersonSurname} (mail: {mail}) nije " +
                            $"sudionik odabranog dogadaja");
                    choice = Back("Lista sudionika dogadaja je uredena", false);
                }
            }
                
        }
        if (choice != "*")
        {
            string idInsert = Answer("Unesi id eventa kojeg zelis obrisati");
            if (ListIdStrings(events).Contains(idInsert) == false) { choice = Back("Ne postoji dogadaj s tim ID-om", true); };
            if (choice != "*")
            {
                foreach (var ev in events)
                    if (ev.Id.ToString() == idInsert && ev.DateStart > DateTime.Now)
                    {
                        string ans = Answer($"Jesi li siguran da zelis izbrisati dogadaj: {ev.EventName}? y/n");
                        if (ans != "y") { choice = Back("Odustali ste od brisanja eventa", true); }
                        if (choice != "*")
                        {
                            List<Osoba> invitedPeople = ReturnInvitedPeople(osobe, ev.ReturnMails());
                            events.Remove(ev);
                            foreach (var os in invitedPeople)
                                os.RemovePresence(ev.Id);
                            Console.WriteLine($"Uspjesno ste izbrisali event {ev.EventName}");
                            break;
                        }
                    }
            }
            choice = "*";
        }
    }
    if (choice != "*")
    {
        Console.Clear();
        foreach (var ev in events)
            if (CheckIsActive(ev) == true)
            {
                Console.WriteLine($"\nAktivan event: \nId eventa: {ev.Id} \nNaziv eventa-{ev.EventName}, Lokacija-{ev.Location}, " +
                        $"Ends in-{decimal.Round((decimal)(ev.DateEnd - DateTime.Now).TotalDays, 1)} dana \nPopis sudionika: ");
                DisplayParticipants(ev, osobe);
            }
        string choice1 = Answer("\n1 - Zabilježi neprisutnosti \n2 - Povratak na glavni menu");
        if (choice1 != "1" && choice1 != "2") { choice = Back("Neispravan unos", true); }
        if (choice1 != "1" && choice != "*") { choice = Back("", true); }
        if (choice != "*")
        {
            string idInsert = Answer("Unesi id eventa na kojem zelis zabiljeziti prisutnosti");
            if (ListIdStrings(events).Contains(idInsert) == false && choice != "*") { choice = Back("Ne postoji dogadaj s tim ID-om", true); };
            if (choice != "*")
            {
                string emails = Answer("Unesi emailove osoba koje nisu bile prisutne");
                List<string> mails = ReturnMail(emails);
                foreach (var ev in events)
                    foreach (var mail in mails)
                    {
                        if (ev.ReturnMails().Contains(mail) == false && CheckIsActive(ev) == true && ev.Id.ToString() == idInsert)
                            Console.WriteLine($"Osoba s mailom {mail} nije sudionik eventa {ev.EventName}");
                        else if (ev.ReturnMails().Contains(mail) == true && CheckIsActive(ev) == true && ev.Id.ToString() == idInsert)
                            ReturnPerson(mail, osobe).RemovePresence(ev.Id);
                    }
                choice = Back("", false);
            }
        }
    }
}
static List<Osoba> UploadFirst10People()
{
    List<string> personSurname = new List<string> { "Munitic", "Krce", "Marasovic", "Pervan", "Babic", "Juric", "Klarica", "Maras", "Stipic", "Matic" };
    List<string> personNames = new List<string> { "Ivana", "Anita", "Mia", "Hajdi", "Ante", "Klara", "Mia", "Andela", "Stjepan", "Mate" };
    List<string> personEmail = new List<string> { "ivanam@gmail.com", "anitak@gmail.com", "miam@gmail.com" , "hajdip@gmail.com",
                "anteb@gmail.com", "klaraj@gmail.com", "miak@gmail.com", "andelam@gmail.com","stjepans@gmail.com",  "matem@gmail.com"};
    List<Osoba> osobe = new List<Osoba>();
    for (int i = 0; i < 10; i++)
    {
        var osoba = new Osoba(personSurname[i], personEmail[i]) { PersonName = personNames[i] };
        osobe.Add(osoba);
    }
    return osobe;
}
static List<Event> UploadFirst5Events(List<Osoba> osobe)
{
    List<string> mailstrings = new List<string> { "anitak@gmail.com hajdip@gmail.com klaraj@gmail.com miak@gmail.com" ,
                "ivanam@gmail.com anitak@gmail.com hajdip@gmail.com",
                "miam@gmail.com anteb@gmail.com matem@gmail.com andelam@gmail.com",
                "hajdip@gmail.com ivanam@gmail.com stjepans@gmail.com matem@gmail.com",
                "anteb@gmail.com ivanam@gmail.com anitak@gmail.com"
            };
    List<string> eventNames = new List<string> { "Krizma", "Pricest", "Vjencanje", "Rodendan", "Promocija" };
    List<string> eventLocations = new List<string> { "Crkva na Visokoj", "Crkva u Varosu", "Crkva na Mejama", "Stan", "Fakultet" };
    List<DateTime> eventDateStarts = new List<DateTime> { new DateTime(2022, 5, 20), new DateTime(2023, 4, 28), DateTime.Now.AddSeconds(1), new DateTime(2022, 12, 30), new DateTime(2022, 9, 30) };
    List<DateTime> eventDateEnds = new List<DateTime> { new DateTime(2023, 5, 21), new DateTime(2023, 4, 30), DateTime.Now.AddMinutes(1), new DateTime(2023, 1, 1), new DateTime(2022, 10, 1) };
    List<Event> events = new List<Event>();
    for (int i = 0; i < 5; i++)
        events.Add(CreateEvent(eventNames[i], eventLocations[i], eventDateStarts[i], eventDateEnds[i], ReturnMail(mailstrings[i]), ReturnInvitedPeople(osobe, ReturnMail(mailstrings[i]))));
    return events;
}
static string Back(string str, bool ErasePrevious)
{
    if (ErasePrevious == true) { Console.Clear(); 
        Console.WriteLine(str);
        return "*";
    }
    Console.WriteLine(str);
    return "*";
}
static string Answer(string question)
{
    Console.WriteLine(question);
    return Console.ReadLine();
}
static string InsertComma(List<string> mailovi)
{
    string stringforOutput = "";
    foreach (var mail in mailovi)
        stringforOutput = stringforOutput + ',' + ' ' + mail;
    return stringforOutput.TrimStart(',', ' ');
}
static bool CheckIsActive(Event ev)
{
    if (DateTime.Now < ev.DateStart || DateTime.Now > ev.DateEnd) { return false; }
    return true;
}
static void DisplayParticipants(Event ev, List<Osoba> osobe)
{
    List<string> mails = new List<string>();
    foreach (var os in osobe)
        if (ev.ReturnMails().Contains(os.Email))
            mails.Add(os.Email);
    Console.WriteLine(InsertComma(mails));
}
static List<string> ListIdStrings(List<Event> events)
{
    List<string> listEventsIdString = new List<string>();
    foreach (var ev in events)
        listEventsIdString.Add(ev.Id.ToString());
    return listEventsIdString; 
}
static List<string> ReturnMail(string mails)
{
    List<string> mailovi = new List<string>();
    string mail = "";
    foreach (char znak in mails + ' ')
    {
        if (znak == ' ')
        {
            mailovi.Add(mail);
            mail = "";
        }
        else
            mail = mail + znak;
    }
    return mailovi;
}
static Osoba ReturnPerson(string mail, List<Osoba> osobe)
{
    foreach (var os in osobe)
        if (mail == os.Email) return os;
    return null;
}
static List<Osoba> ReturnInvitedPeople(List<Osoba> osobe, List<string> mails)
{
    List<Osoba> invitedPeople = new List<Osoba>();
    foreach (var os in osobe)
        foreach (var mail in mails)
            if (os.Email == mail)
                invitedPeople.Add(os);
    return invitedPeople;
}
static string FinishedEventsChosen(List<Event> events, List<Osoba> osobe)
{
    foreach (var ev in events)
        if (ev.DateEnd < DateTime.Now)
        {
            Console.WriteLine($"\n{ev.EventName}-{ev.Location}-prije {decimal.Round((decimal)(DateTime.Now - ev.DateEnd).TotalDays)} dana-trajalo {decimal.Round((decimal)(ev.DateEnd - ev.DateStart).TotalHours, 1)} sata");
            List<string> prisutni = new List<string>();
            List<string> nisuPrisutni = new List<string>();

            foreach (var os in osobe)
                if (os.CheckHasEvent(ev.Id) == "prisutan")
                    prisutni.Add(os.Email);
                else if(os.CheckHasEvent(ev.Id) == "nije prisutan")
                    nisuPrisutni.Add(os.Email);

            Console.WriteLine("Prisutni: " + InsertComma(prisutni));
            Console.WriteLine("Nisu prisutni: " + InsertComma(nisuPrisutni));
        }
    return "*";
}
static Event CreateEvent(string name, string location, DateTime dateSt, DateTime dateEnd, List<string> mailList, List<Osoba> invited)
{
    Event newEvent = new Event(mailList)
    {
        EventName = name,
        Location = location,
        DateStart = dateSt,
        DateEnd = dateEnd
    };
    foreach (var os in invited)
        os.ChangePresence(mailList, newEvent.Id);
    return newEvent;
}