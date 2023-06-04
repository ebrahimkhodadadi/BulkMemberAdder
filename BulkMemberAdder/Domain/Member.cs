
using Bogus;

namespace BulkMemberAdder.Domain
{
    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }

        public static Faker<Member> GenerateFakeMember()
        {
            return new Faker<Member>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Mobile, f => f.Person.Phone);
        }
    }
}
