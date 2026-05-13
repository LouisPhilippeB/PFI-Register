using EmailHandling;
using Models;

namespace DAL
{
    public sealed class DB
    {
        private static readonly DB instance = new DB();
        public static DB Instance { get { return instance; } }

        static public UsersRepository Users { get; set; }
            = new UsersRepository();

        static public NotificationsRepository Notifications { get; set; }
            = new NotificationsRepository();

        static public LoginsRepository Logins { get; set; }
            = new LoginsRepository();

        static public EventsRepository Events { get; set; }
            = new EventsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();

        static public StudentsRepository Students { get; set; }
            = new StudentsRepository();

        static public CoursesRepository Courses { get; set; }
            = new CoursesRepository();

        static public TeachersRepository Teachers { get; set; }
            = new TeachersRepository();

        static public RegistrationsRepository Registrations { get; set; }
            = new RegistrationsRepository();

        static public AllocationsRepository Allocations { get; set; }
            = new AllocationsRepository();
    }
}