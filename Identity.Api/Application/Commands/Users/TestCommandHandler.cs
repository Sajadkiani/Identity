using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

//TODO: just for use in consumer 
public class TestCommandHandler : IRequestHandler<TestCommand, bool>
{

    public Task<bool> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}
