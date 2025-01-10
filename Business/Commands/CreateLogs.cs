using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateLogs : IRequest<CreateLogsResult>
    {
        public required Logs Logs { get; set; }
    }

    public class CreateLogsHandler(StargateContext context) : IRequestHandler<CreateLogs, CreateLogsResult>
    {
        private readonly StargateContext _context = context;

        public async Task<CreateLogsResult> Handle(CreateLogs request, CancellationToken cancellationToken)
        {
            var newLog = new Logs()
            {
                LogDateTime = request.Logs.LogDateTime,
                LogMessage = request.Logs.LogMessage,
                LogException = request.Logs.LogException,
                LogLevel = request.Logs.LogLevel
            };

            await _context.Logs.AddAsync(newLog, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new CreateLogsResult()
            {
                Id = newLog.Id
            };
        }
    }

    public class CreateLogsResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
