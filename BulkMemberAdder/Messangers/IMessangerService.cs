
using BulkMemberAdder.Domain;

namespace BulkMemberAdder.Messangers;

public interface IMessangerService
{
    Task Start(List<Member> memberList);
    void Stop();
}
